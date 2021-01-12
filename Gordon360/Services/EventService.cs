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
using System.Xml.Linq;


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

            return result
                .Where(x => x.CHTermCD.Trim().Equals(term))
                .Join(
                    GetAllEvents(),
                    c => c.LiveID,
                    e => e.Event_ID,
                    (c, e) => new AttendedEventViewModel(e, c)
                );
        }
    }
}