using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gordon360.Services
{
    /// <summary>
    /// Service that allows for event control
    /// </summary>
    public class EventService : IEventService
    {
        // See UnitOfWork class
        private IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches a single account record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The person's gordon id</param>
        /// <returns>AccountViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ChapelEvent)]
        public ChapelEventViewModel Get(string id)
        {
            var query = _unitOfWork.ChapelEventRepository.GetById(id);
            if (query == null)
            {
                // Custom Exception is thrown that will be cauth in the controller Exception filter.
                throw new ResourceNotFoundException() { ExceptionMessage = "The event was not found." };
            }
            ChapelEventViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ChapelEvent)]
        public IEnumerable<ChapelEventViewModel> GetAll()
        {
            var query = _unitOfWork.ChapelEventRepository.GetAll();
            var result = query.Select<ChapelEvent, ChapelEventViewModel>(x => x); //Map the database model to a more presentable version (a ViewModel)
            return result;
        }

        /// <summary>
        /// Return a Single Event JObject from Live25
        /// </summary>
        /// <returns></returns>
        public JObject GetLiveEvent(string EventID)
        {
            var requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_id=" + EventID + "&scope=extended";

            using (WebClient client = new WebClient())
            {
                MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8 ))
                {
                    string readContents = streamReader.ReadToEnd();
                    var data = (JObject)JsonConvert.DeserializeObject(readContents);
                    return data;
                }
            }
        }

        /// <summary>
        /// Fetches an event from Live25, and parses it into a smaller ViewModel to send off to the front end
        /// </summary>
        /// <param name="EventID">The Event ID for an Event</param>
        /// <returns>EventViewModel</returns>
        public EventViewModel GetEvent(string EventID)
        {
            EventViewModel vm = new EventViewModel(GetLiveEvent(EventID));
            return vm;
        }

        /// <summary>
        /// Fetches the event with the specified ID (from Gordon's Database)
        /// </summary>
        /// <param name="CHEventID">The email address associated with the account.</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ChapelEvent)]
        public ChapelEventViewModel GetChapelEventByChapelEventID(string CHEventID)
        {
            var query = _unitOfWork.ChapelEventRepository.FirstOrDefault(x => x.CHEventID == CHEventID);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The event was not found." };
            }
            ChapelEventViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }

        /// <summary>
        /// Returns all attended events for a student
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public IEnumerable<ChapelEventViewModel> GetAllForStudent(string user_name)
        {
            // Confirm that student exists
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.AD_Username.Trim() == user_name.Trim()).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            // Declare the variables used
            var idParam = new SqlParameter("@STU_USERNAME", user_name.Trim());
            // Run the query, which returns an iterable json list 
            var result = RawSqlQuery<ChapelEventViewModel>.query("EVENTS_BY_STUDENT_ID @STU_USERNAME", idParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            // Trim white space off of results
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ROWID = x.ROWID;
                trim.CHBarEventID = x.CHBarEventID.Trim();
                trim.CHEventID = x.CHEventID.Trim();
                trim.CHCheckerID = x.CHCheckerID.Trim();
                trim.CHDate = x.CHDate;
                trim.CHTermCD = x.CHTermCD.Trim();
                trim.Required = x.Required;
                return trim;
            });
            return trimmedResult;
        }

        /// <summary>
        /// Returns all attended events for a student
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <param name="term"> The current term</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public IEnumerable<ChapelEventViewModel> GetEventsForStudentByTerm(string user_name, string term)
        {
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.AD_Username.Trim() == user_name.Trim()).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            // Declare the variables used
            var idParam = new SqlParameter("@STU_USERNAME", user_name.Trim());
            
            // Run the stored query  and return an iterable list of objects
            var result = RawSqlQuery<ChapelEventViewModel>.query("EVENTS_BY_STUDENT_ID @STU_USERNAME", idParam );

            // Confirm that result is not empty
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            // Filter out the events that are part of the specified term, based on the attribute specified, then sort by Date
            result = result.Where(x => x.CHTermCD.Trim().Equals(term)).OrderByDescending(x => x.CHDate);

            
            // Trim the white space off of each entry
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ROWID = x.ROWID;
                trim.CHBarEventID = x.CHBarEventID.Trim();
                trim.CHEventID = x.CHEventID.Trim();
                trim.CHCheckerID = x.CHCheckerID.Trim();
                trim.CHDate = x.CHDate.Value.Add(x.CHTime.Value.TimeOfDay);
                trim.CHTime = x.CHTime;
                trim.CHTermCD = x.CHTermCD.Trim();
                trim.Required = x.Required;
                return trim;
            });

            return trimmedResult;
        }

    }
}