using System.Collections.Generic;
using Gordon360.Models.ViewModels;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Database.CCT;

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
        /// <param name="id">The session code.</param>
        /// <returns>A SessionViewModel if found, null if not found.</returns>
        public SessionViewModel Get(string id)
        {
            var query = _context.CM_SESSION_MSTR.Find(id);
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
            return (IEnumerable<SessionViewModel>)_context.CM_SESSION_MSTR;
        }

        
    }
}