using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
        /// Return a Single Event JObject from Live25
        /// </summary>
        /// <returns></returns>
        public JObject GetLiveEvent(string EventID, string type) {

            // Return a list of all chapel events
            if (EventID == "Chapel")
            {
              
                // Set our api route and fill in the event information we would like
                var requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&category_id=85&state=2&scope=extended";
                using (WebClient client = new WebClient())
                {
                    // Commit contents of the request to temporary memory
                    MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                    // Begin to read contents with correct encoding
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string readContents = streamReader.ReadToEnd();
                        // Parse the data into a json object
                        var data = (JObject)JsonConvert.DeserializeObject(readContents);
                        return data;
                    }
                }
            }
            // If the type is "s", then it is a single event request, "m" is multiple event IDs
            else if (type == "s" || type == "S" || type == "m" || type == "M")
            {   
                if (type == "m" || type == "M")
                {
                    if (EventID.Contains('$'))
                    {
                        EventID = EventID.Replace('$', '+');
                    }
                }
                // Set our api route and fill in the event information we would like
                var requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_id="+EventID+"&state=2&scope=extended";
                using (WebClient client = new WebClient())
                {
                    // Commit contents of the request to temporary memory
                    MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                    // Begin to read contents with correct encoding
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string readContents = streamReader.ReadToEnd();
                        // Parse the data into a json object
                        var data = (JObject)JsonConvert.DeserializeObject(readContents);
                        return data;
                    }
                }
            }
            // If it is a type "t", then it is a search for a type"
            // Same logic is used as above 
            else if (type == "t" || type == "T")
            {
                if (EventID.Contains('$'))
                {
                    EventID = EventID.Replace('$', '+');
                }
                var requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_type_id=" + EventID + "&state=2&scope=extended";
                using (WebClient client = new WebClient())
                {
                    MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string readContents = streamReader.ReadToEnd();
                        var data = (JObject)JsonConvert.DeserializeObject(readContents);
                        return data;
                    }
                }
            }
            // If the input is incorrect, throw an appropriate error
            else
            {
                throw new Exception("Invalid type!");
            }
        }

        /// <summary>
        /// Fetches an event from Live25, and parses it into a smaller ViewModel to send off to the front end
        /// </summary>
        /// <param name="EventID">The Event ID for an Event</param>
        /// <param name="type"> the type of item being parsed</param>
        /// <returns>EventViewModel</returns>
        public IEnumerable<EventViewModel> GetEvents(string EventID, string type)
        {
            // Use defined function to query 25Live
            JObject events = GetLiveEvent(EventID, type);
            // Initiate a list to contain the events
            List<EventViewModel> list = new List<EventViewModel>();

            // If the event key is of type array, we know there are more than one events nested within, so check for that here
            if (events.SelectToken("events.event").Type != JTokenType.Array)
            {
                EventViewModel vm = new EventViewModel(events, 0, true);
                list.Add(vm);
                IEnumerable<EventViewModel> result = list.AsEnumerable<EventViewModel>();
                return result;
            }

            // Otherwise, we treat it as a json object containing multiple events 
            else
            {
                // Determine how many events exist within the query contents
                int index = events["events"]["event"].Count();
                // Iterate through the JSon Object of events and add them to the list
                for (int y = 0; y < index; y++)
                {
                    EventViewModel vm = new EventViewModel(events, y, false);
              
                    list.Add(vm);
                }

                IEnumerable<EventViewModel> result = list.AsEnumerable<EventViewModel>();
                return result;
            }

        }


        /// <summary>
        /// Returns all attended events for a student
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public IEnumerable<AttendedEventViewModel> GetAllForStudent(string user_name)
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
                throw new ResourceNotFoundException() { ExceptionMessage = "No events attended yet!" };
            }

            // A list to hold each combined event until we finish
            List<AttendedEventViewModel> list = new List<AttendedEventViewModel>();
            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.CHEventID));
            // Return all events attended by the student from 25Live
            IEnumerable<EventViewModel> events = GetEvents(joined, "m");

            foreach (var c in result)
            {
                // Find the event with the same ID as the attended event
                EventViewModel l = events.ToList().Find(x => x.Event_ID == c.CHEventID);
                // Combine the two view models
                AttendedEventViewModel combine = new AttendedEventViewModel(l, c);
                // Add to the list we made earlier
                list.Add(combine);
            }
            // Convert the list to an IEnumerable 
            IEnumerable<AttendedEventViewModel> vm = list.AsEnumerable<AttendedEventViewModel>();
            return vm;
        }


        /// <summary>
        /// Returns all attended events for a student in a specific term
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <param name="term"> The current term</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public IEnumerable<AttendedEventViewModel> GetEventsForStudentByTerm(string user_name, string term)
        {
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.AD_Username.Trim() == user_name.Trim()).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            // Declare the variables used
            var idParam = new SqlParameter("@STU_USERNAME", user_name.Trim());

            // Run the stored query  and return an iterable list of objects
            var result = RawSqlQuery<ChapelEventViewModel>.query("EVENTS_BY_STUDENT_ID @STU_USERNAME", idParam);

            // Confirm that result is not empty
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            // Filter out the events that are part of the specified term, based on the attribute specified, then sort by Date
            result = result.Where(x => x.CHTermCD.Trim().Equals(term)).OrderByDescending(x => x.CHDate);

            // A list to hold each combined event until we finish
            List<AttendedEventViewModel> list = new List<AttendedEventViewModel>();
            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.CHEventID));
            // If there exists an event, pull info for it!
            if (joined != "")
            {
                // Return all events attended by the student from 25Live
                IEnumerable<EventViewModel> events = GetEvents(joined, "m");

                foreach (var c in result)
                {
                    // Find the event with the same ID as the attended event
                    EventViewModel l = events.ToList().Find(x => x.Event_ID == c.CHEventID);
                    // Combine the two view models
                    AttendedEventViewModel combine = new AttendedEventViewModel(l, c);
                    // Add to the list we made earlier
                    list.Add(combine);
                }
                // Convert the list to an IEnumerable 
                IEnumerable<AttendedEventViewModel> vm = list.AsEnumerable<AttendedEventViewModel>();

                return vm;
            }
            // Since the string "joined" is empty, we can assume there are no attended events this term 
            else
            {
                throw new Exception("No events attended this term!");
            }

        }

    }
}