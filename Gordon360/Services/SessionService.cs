using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate data transactions between the Controller and the database model.
    /// </summary>
    public class SessionService : ISessionService
    {
        private CCTContext _context;

        public SessionService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the session record whose sesssion code matches the parameter.
        /// </summary>
        /// <param name="sessionCode">The session code.</param>
        /// <returns>A SessionViewModel if found, null if not found.</returns>
        public SessionViewModel Get(string sessionCode)
        {
            var query = _context.CM_SESSION_MSTR.Where(s => s.SESS_CDE == sessionCode).FirstOrDefault();
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            return query;
        }


        /// <summary>
        /// Fetches all the session records from the database.
        /// </summary>
        /// <returns>A SessionViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SessionViewModel> GetAll()
        {
            return _context.CM_SESSION_MSTR.Select<CM_SESSION_MSTR, SessionViewModel>(s => s);
        }


    }
}