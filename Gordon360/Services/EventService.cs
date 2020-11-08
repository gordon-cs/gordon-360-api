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


/// <summary>
/// We use this service to pull data from 25Live as well as parsing it
/// For the most part, we use the data which is pulled and cached in the Startup.cs file
/// However, there are functions included in this service to manually pull data from 25Live
/// </summary>
namespace Gordon360.Services
{
    /// <summary>
    /// Service that allows for event control
    /// </summary>
    public class EventService : IEventService
    {
        // See UnitOfWork class
        private IUnitOfWork _unitOfWork;

        // Set the namespace for XML Paths
        private XNamespace r25 = "http://www.collegenet.com/r25";

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Helper function to set the route we are making our request to in 25Live
        /// </summary>
        /// <param name="EventID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetRoute(string EventID, string type) 
        // all these '&event_type_id=<somId+someId>'s might need to change, actually we don't use the one that just gets a single type
        {
            // Get the current year
            string year = Helpers.GetFirstEventDate();
            // Make an empty url string
            string requestUrl = "";

            // Return a list of all chapel events
            if (EventID == "All")
            {
                // Set our api route and fill in the event information we would like
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=14+19+28+57&state=2&end_after=" + year + "&scope=extended";
            }

            // If the type is "s", then it is a single event request, "m" is multiple event IDs
            else if (type == "s" || type == "S" || type == "m" || type == "M")
            {
                // For 25Live, additional types must be separated by "+", so we replace the $ with "+"s here
                if (type == "m" || type == "M")
                {
                    if (EventID.Contains('$'))
                    {
                        EventID = EventID.Replace('$', '+');
                    }
                }
                // Set our api route and fill in the event information we would like
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_id=" + EventID + "&scope=extended";

            }
            // If it is a type "t", then it is a search for a type"
            // Same logic is used as above 
            else if (type == "t" || type == "T")
            {
                if (EventID.Contains('$'))
                {
                    EventID = EventID.Replace('$', '+');
                }
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=" + EventID + "&state=2&end_after=" + year + "&scope=extended";
            }

            return requestUrl;
        }


        /// <summary>
        /// Return a Single Event JObject from Live25
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventViewModel> GetSpecificEvents(string EventID, string type)
        {

            // Get API Route for 25 Live
            string requestUrl = GetRoute(EventID, type);
            // Commit contents of the request to temporary memory
            XDocument xmlDoc = Helpers.GetLiveStream(requestUrl);

            // Pull out the nodes for events
            IEnumerable<XElement> events = xmlDoc.Descendants(r25 +"event");

            // Convert to iterable list containing just the pieces we need
            List<EventViewModel> stuff = new List<EventViewModel>();

            // Convert each element into a viewmodel for events          
            foreach (XElement n in events)
            {
                EventViewModel vm = new EventViewModel(n);
                stuff.Add(vm);
            }

            return stuff.AsEnumerable<EventViewModel>();
            
        }
        
        /// <summary>
        /// Access the memory stream created by the cached task and parse it into events
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public IEnumerable<EventViewModel> GetAllEvents (XDocument xmlDoc)
        {

                // Pull out the nodes for events
                IEnumerable<XElement> events = xmlDoc.Descendants(r25 + "event");

                // Convert to iterable list containing just the pieces we need
                List<EventViewModel> stuff = new List<EventViewModel>();

                // Convert each element into a viewmodel for events          
                foreach (XElement n in events)
                {
                    EventViewModel vm = new EventViewModel(n);
                    stuff.Add(vm);
                }

                return stuff.AsEnumerable<EventViewModel>();
            
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

            // Create an empty event view model, to use just in case
            EventViewModel whoops = null;

            // Create an empty list of events, to use just in case
            IEnumerable<EventViewModel> events = null;

            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.LiveID));

            // Attempt to return all events attended by the student from 25Live
            events = GetAllEvents(Data.AllEvents);

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

                // Bring the two together into an AttendedEventViewModel
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
            }
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
            events = GetAllEvents(Data.AllEvents);

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
                }
            }

            // Attempt to convert the list to a ViewModel we can return
            try { 
                vm = list.AsEnumerable<AttendedEventViewModel>();
            }

            catch (Exception c)
            {
                // Do Nothing -- Let the Front End handle a return containing 0 Events
            }
            return vm;
        }
    }
}