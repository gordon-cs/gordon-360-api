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
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;



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

        public string GetDay()
        { 
        // We need to determine what the current date is
          DateTime today = DateTime.Today;
          if(today.Month < 06)
            {
                return (today.Year - 1).ToString();
            }
          else if(today.Month > 07)
            {
                return (today.Year).ToString();
            }
          return today.Year.ToString();
         }

        /// <summary>
        /// Return a Single Event JObject from Live25
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> GetLiveEvent(string EventID, string type)
        {
            // Get the current year
            string year = GetDay();
            // Make an empty url string
            string requestUrl = "";

            // Return a list of all chapel events
            if (EventID == "All")
            {
                // Set our api route and fill in the event information we would like
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=10+12+13+14+16+17+18+19+51+20+21+22+23+24+25+29+30+33+41&state=2&end_after=" + year + "0820&scope=extended";
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
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_id=" + EventID + "&state=2&end_after=" + year + "0820&scope=extended";

            }
            // If it is a type "t", then it is a search for a type"
            // Same logic is used as above 
            else if (type == "t" || type == "T")
            {
                if (EventID.Contains('$'))
                {
                    EventID = EventID.Replace('$', '+');
                }
                requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_type_id=" + EventID + "&state=2&end_after=" + year + "0820&scope=extended";
            }

            using (WebClient client = new WebClient())
            {
                // Commit contents of the request to temporary memory
                MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                // Begin to read contents with correct encoding
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Create a string of content
                    string content = reader.ReadToEnd();
                    // Load the data into an XmlDocument
                    XDocument xmlDoc = XDocument.Parse(content);

                    // Pull out the nodes for events

                    IEnumerable<XElement> events = xmlDoc.Descendants(r25 +"event");
                    return events;

                }
            } 
        }
            
        
        /// <summary>
        ///  Converts events from XML Nodes to EventViewModesl
        /// </summary>
        /// <param name="nodeList"> The XML Node List</param>
        public IEnumerable<EventViewModel> GetAllEvents(IEnumerable<XElement> nodeList)
        {
            List<EventViewModel> stuff = new List<EventViewModel>();
            foreach (XElement n in nodeList)
            {
                
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

            // Create an empty event view model, to use just in case we cannot find the event in 25Live



            // Fill it with empty strings

            // Create an empty list of events, to use just in case
            IEnumerable<EventViewModel> events = null;

            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.CHEventID));


            foreach (var c in result)
            {
                try
                {
                    // Find the event with the same ID as the attended event
                    EventViewModel l = events.ToList().Find(x => x.Event_ID == c.CHEventID);
                }
                catch
                {
                    // Ignore issue and continue to iterate
                }
                AttendedEventViewModel combine = null;
                // Add to the list we made earlier
                list.Add(combine);

            }
            // In the database, the time and date are stored as separate datetime objects, here we combine them into one
            foreach (var v in list)
            {
                v.CHDate = v.CHDate.Value.Add(v.CHTime.Value.TimeOfDay);
            }

            // Convert the list to an IEnumerable 
            IEnumerable<AttendedEventViewModel> vm = list.AsEnumerable<AttendedEventViewModel>();

            // Returm the iterable viewmodel 
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

            // Create an empty event view model, to use just in case


            // Fill it with empty strings

            // Create an empty list of events, to use just in case
            IEnumerable<EventViewModel> events = null;

            // Get a list of every attended event, to send over to 25Live
            string joined = string.Join("+", result.Select(x => x.CHEventID));

            // Attempt to return all events attended by the student from 25Live


            foreach (var c in result)
            {
                try
                {
                    // Find the event with the same ID as the attended event
                    EventViewModel l = events.ToList().Find(x => x.Event_ID == c.CHEventID);
                }
                catch
                {
                    // Ignore issue and continue to iterate
                }
                AttendedEventViewModel combine = null;
                // Add to the list we made earlier
                list.Add(combine);

            }
            // In the database, the time and date are stored as separate datetime objects, here we combine them into one
            foreach (var v in list)
            {
                v.CHDate = v.CHDate.Value.Add(v.CHTime.Value.TimeOfDay);
            }

            if (list.Any<AttendedEventViewModel>())
            {
                // Convert the list to an IEnumerable 
                IEnumerable<AttendedEventViewModel> vm = list.AsEnumerable<AttendedEventViewModel>();
                // Returm the iterable viewmodel 
                return vm;
            }
            else
            {
                throw new Exception("No attendance information for this term");
            }

        }
    }
}