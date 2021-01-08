using System;
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
using Gordon360.Static.Data;
using Gordon360.Static.Methods;
using System.Xml.Linq;


// <summary>
// We use this service to pull data from 25Live as well as parsing it
// For the most part, we use the data which is pulled and cached in the Startup.cs file
// However, there are functions included in this service to manually pull data from 25Live
// </summary>
namespace Gordon360.Services
{
    /// <summary>
    /// Service that allows for event control
    /// </summary>
    public class EventService : IEventService
    {
        // See UnitOfWork class
        private readonly IUnitOfWork _unitOfWork;

        // Set the namespace for XML Paths
        private readonly XNamespace r25 = "http://www.collegenet.com/r25";

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Access the memory stream created by the cached task and parse it into events
        /// </summary>
        /// <returns>All events for the current academic year.</returns>
        public IEnumerable<EventViewModel> GetAllEvents()
        {

            return Data.AllEvents.Descendants(r25 + "event").Select(e => new EventViewModel(e));
        }

        /// <summary>
        /// Select only events that are marked for Public promotion
        /// </summary>
        /// <returns>All Public Events</returns>
        public IEnumerable<EventViewModel> GetPublicEvents()
        {
            return GetAllEvents().Where(e => e.Requirements.Any(r => r.RequirementID == "3"));
        }

        /// <summary>
        /// Select only events that are Approved to give CLAW credit
        /// </summary>
        /// <returns>All CLAW Events</returns>
        public IEnumerable<EventViewModel> GetCLAWEvents()
        {
            return GetAllEvents().Where(e => e.Categories.Any(c => c.CategoryID == "85"));
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

            // This is the only real difference between the last method and this one
            // Filter out the events that are part of the specified term, based on the attribute specified, then sort by Date
            result = result.Where(x => x.CHTermCD.Trim().Equals(term)).OrderByDescending(x => x.CHDate);

            // A list to hold each combined event until we finish
            List<AttendedEventViewModel> list = new List<AttendedEventViewModel>();

            // Create an empty event view model, to use just in case
            EventViewModel whoops = null;

            // Create an empty list of events, to use just in case
            IEnumerable<EventViewModel> events = null;

            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.LiveID));

            // Attempt to return all events attended by the student from 25Live
            // We use the cached data
            events = GetAllEvents();

            // Loop through each event a student has attended and pull it's corresponding details from 25Live
            foreach (var c in result)
            {
                try
                {
                    // Find the event with the same ID as the attended event
                    EventViewModel l = events.ToList().Find(x => x.Event_ID == c.LiveID);
                    whoops = l;
                }
                catch
                {
                    // Ignore issue and continue to iterate
                }
                AttendedEventViewModel combine = new AttendedEventViewModel(whoops, c);
                // Add to the list we made earlier
                list.Add(combine);

            }

            // Declare an empty AttendedEventViewModel to return in the case of a problem
            IEnumerable<AttendedEventViewModel> vm = null;
            // In the database, the time and date are stored as separate datetime objects, here we combine them into one
            foreach (var v in list)
            {
                try
                {
                    v.CHDate = v.CHDate.Value.Add(v.CHTime.Value.TimeOfDay);
                }
                catch (InvalidOperationException e)
                {
                    // time value is null -- don't worry bout it
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            // Attempt to convert the list to a ViewModel we can return
            try
            {
                vm = list.AsEnumerable<AttendedEventViewModel>();
            }

            catch (Exception c)
            {
                // Do Nothing -- Let the Front End handle a return containing 0 Events
                System.Diagnostics.Debug.WriteLine(c.Message);
            }
            return vm;
        }
    }
}