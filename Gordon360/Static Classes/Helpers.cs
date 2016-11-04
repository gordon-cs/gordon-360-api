using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Static.Methods
{
    /// <summary>
    /// Service class for methods that are shared between all services.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Service method that gets the current session we are in.
        /// </summary>
        /// <returns>SessionViewModel of the current session. If no session is found for our current date, returns null.</returns>
        public static SessionViewModel GetCurrentSession()
        {
            var tempUnitOfWork = new UnitOfWork();

            // Filters the sessions to make sure they have dates.
            var notNullQuery = tempUnitOfWork.SessionRepository.Where(x => x.SESS_BEGN_DTE.HasValue && x.SESS_END_DTE.HasValue);
            var currentSession = notNullQuery.OrderByDescending(x => x.SESS_BEGN_DTE.Value).FirstOrDefault();
            SessionViewModel result = currentSession;
            return result; ;
        }

        public static string GetLeaderRoleCodes()
        {
            return "LEAD";
        }

        public static string GetTranscriptWorthyRoles()
        {
            //string[] transcriptWorthyRoles = { "CAPT", "CODIR", "CORD", "DIREC", "PRES", "VICEC", "VICEP", "AC", "RA1", "RA2","RA3", "SEC" };
            return "LEAD";
        }
    }
}