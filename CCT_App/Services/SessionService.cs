using System;
using System.Collections.Generic;
using System.Linq;
using CCT_App.Models;
using CCT_App.Models.ViewModels;
using CCT_App.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace CCT_App.Services
{
    /// <summary>
    /// Service class to facilitate data transactions between the Controller and the database model.
    /// </summary>
    public class SessionService : ISessionService
    {
        private IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get the session record whose sesssion code matches the parameter.
        /// </summary>
        /// <param name="id">The session code.</param>
        /// <returns>A SessionViewModel if found, null if not found.</returns>
        public SessionViewModel Get(string id)
        {
            var query = _unitOfWork.SessionRepository.GetById(id);
            SessionViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches the Activities that are active during the session whose code is specified as parameter.
        /// </summary>
        /// <param name="id">The session code</param>
        /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ActivityViewModel> GetActivitiesForSession(string id)
        {
            var query = _unitOfWork.ActivityRepository.ExecWithStoredProcedure
                ("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", 
                new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = id });
            var result = query.Select<ACT_CLUB_DEF, ActivityViewModel>(x => x);
            return result;
        }


        /// <summary>
        /// Fetches all the session records from the database.
        /// </summary>
        /// <returns>A SessionViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SessionViewModel> GetAll()
        {
            var query = _unitOfWork.SessionRepository.GetAll();
            var result = query.Select<CM_SESSION_MSTR, SessionViewModel>(x => x);
            return result;
        }

        /// <summary>
        /// Service method that gets the current session we are in.
        /// </summary>
        /// <returns>SessionViewModel of the current session. If no session is found for our current date, returns null.</returns>
        public SessionViewModel GetCurrentSession()
        {

            var currentDateTime = DateTime.Now;
            var notNullQuery = _unitOfWork.SessionRepository.Where(x => x.SESS_BEGN_DTE.HasValue && x.SESS_END_DTE.HasValue);
            var currentSession = notNullQuery.Where( x => 
                (currentDateTime.CompareTo(x.SESS_BEGN_DTE.Value) > 0)
                && 
                (currentDateTime.CompareTo(x.SESS_END_DTE.Value) < 0) )
                .OrderByDescending(x => x.SESS_BEGN_DTE.Value)
                .AsEnumerable()
                .LastOrDefault();
            return currentSession;
        }
    }
}