using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace CCT_App.Services
{
    public class SessionService : ISessionService
    {
        private IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CM_SESSION_MSTR Get(string id)
        {
            var result = _unitOfWork.SessionRepository.GetById(id);
            return result;
        }

        public IEnumerable<ACT_CLUB_DEF> GetActivitiesForSession(string id)
        {
            var activitiesInSession = _unitOfWork.ActivityRepository.ExecWithStoredProcedure
                ("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", 
                new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = id });
            return activitiesInSession;
        }

        public IEnumerable<CM_SESSION_MSTR> GetAll()
        {
            var result = _unitOfWork.SessionRepository.GetAll();
            return result;
        }

        public CM_SESSION_MSTR GetCurrentSession()
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