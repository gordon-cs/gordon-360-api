using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Data;
using System.Xml.Linq;
using Gordon360.Models;
using Gordon360.Database.CCT;
using System.Threading.Tasks;

// <summary>
// We use this service to pull data from 25Live as well as parsing it
// The data is retrieved from the cache maintained by Startup.cs
// </summary>
namespace Gordon360.Services
{
    /// <summary>
    /// Service that allows for event control
    /// </summary>
    public class EventService : IEventService
    {
        private readonly CCTContext _context;

        // Set the namespace for XML Paths
        private readonly XNamespace r25 = "http://www.collegenet.com/r25";

        public EventService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Access the memory stream created by the cached task and parse it into events
        /// Splits events with multiple repeated occurrences into individual records for each occurrence
        /// </summary>
        /// <returns>All events for the current academic year.</returns>
        public IEnumerable<EventViewModel> GetAllEvents()
        {
            return Data
                    .AllEvents
                    // Select the event elements
                    .Descendants(EventViewModel.r25 + "event")
                    .SelectMany(
                        // Select occurrences of each events
                        elem => elem.Element(EventViewModel.r25 + "profile")?.Descendants(EventViewModel.r25 + "reservation"),
                        // Map the event and with it's occurrence details o to a new EventViewModel
                        (e, o) => new EventViewModel(e, o)
                    );
        }

        /// <summary>
        /// Select only events that are marked for Public promotion
        /// </summary>
        /// <returns>All Public Events</returns>
        public IEnumerable<EventViewModel> GetPublicEvents()
        {
            return GetAllEvents().Where(e => e.IsPublic);
        }

        /// <summary>
        /// Select only events that are Approved to give CLAW credit
        /// </summary>
        /// <returns>All CLAW Events</returns>
        public IEnumerable<EventViewModel> GetCLAWEvents()
        {
            return GetAllEvents().Where(e => e.HasCLAWCredit);
        }

        /// <summary>
        /// Returns all attended events for a student in a specific term
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <param name="term"> The current term</param>
        /// <returns></returns>
        public async Task<IEnumerable<AttendedEventViewModel>> GetEventsForStudentByTerm(string user_name, string term)
        {
            var studentExists = _context.ACCOUNT.Where(x => x.AD_Username.Trim() == user_name.Trim()).Any();
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            var result = (IEnumerable<ChapelEventViewModel>)await _context.Procedures.EVENTS_BY_STUDENT_IDAsync(user_name);

            // Confirm that result is not empty
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            return result
                .Where(x => x.CHTermCD.Trim().Equals(term))
                .Join(
                    GetAllEvents(),
                    c => c.LiveID,
                    e => e.Event_ID,
                    (c, e) => new AttendedEventViewModel(e, c)
                );
        }

        // <summary>
        /// Access the memory stream created by the cached task and parse it into events
        /// Splits events with multiple repeated occurrences into individual records for each occurrence
        /// </summary>
        /// <returns>All events for the current academic year.</returns>
        public IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetAllEvents()
        {
            var events = Data.AllEvents.Descendants(EventViewModel.r25 + "event").Select(e => new DEPRECATED_EventViewModel(e));
            return events.SelectMany(e => e.Occurrences, (e, o) => new DEPRECATED_EventViewModel(e, o));
        }

        /// <summary>
        /// Select only events that are marked for Public promotion
        /// </summary>
        /// <returns>All Public Events</returns>
        public IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetPublicEvents()
        {
            return DEPRECATED_GetAllEvents().Where(e => e.IsPublic);
        }

        /// <summary>
        /// Select only events that are Approved to give CLAW credit
        /// </summary>
        /// <returns>All CLAW Events</returns>
        public IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetCLAWEvents()
        {
            return DEPRECATED_GetAllEvents().Where(e => e.HasCLAWCredit);
        }

        /// <summary>
        /// Returns all attended events for a student in a specific term
        /// </summary>
        /// <param name="user_name"> The student's ID</param>
        /// <param name="term"> The current term</param>
        /// <returns></returns>
        public IEnumerable<DEPRECATED_AttendedEventViewModel> DEPRECATED_GetEventsForStudentByTerm(string user_name, string term)
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

            return result
                .Where(x => x.CHTermCD.Trim().Equals(term))
                .Join(
                    DEPRECATED_GetAllEvents(),
                    c => c.LiveID,
                    e => e.Event_ID,
                    (c, e) => new DEPRECATED_AttendedEventViewModel(e, c)
                );
        }
    }
}
