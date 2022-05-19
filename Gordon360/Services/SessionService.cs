using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System;
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

        public SessionViewModel GetCurrentSession()
        {
            return _context.CM_SESSION_MSTR.Where(s => DateTime.Now > s.SESS_BEGN_DTE).OrderByDescending(s => s.SESS_BEGN_DTE).First();
        }

        // Return the first day in the current session
        public string GetFirstDay()
        {
            var currentSession = GetCurrentSession();
            DateTime firstDayRaw = currentSession.SessionBeginDate.Value;
            string firstDay = firstDayRaw.ToString("MM/dd/yyyy");
            return firstDay;
        }

        // Return the last day in the current session
        public string GetLastDay()
        {
            var currentSession = GetCurrentSession();
            DateTime lastDayRaw = currentSession.SessionEndDate.Value;
            string lastDay = lastDayRaw.ToString("MM/dd/yyyy");
            return lastDay;
        }

        // Return the days left in the semester, and the total days in the current session
        public double[] GetDaysLeft()
        {
            var currentSession = GetCurrentSession();
            // The end of the current session
            DateTime sessionEnd = currentSession.SessionEndDate.Value;
            DateTime sessionBegin = currentSession.SessionBeginDate.Value;
            // Get todays date
            DateTime startTime = DateTime.Today;
            //Initialize array
            double[] days = new double[2];
            // Days left in semester
            days[0] = (sessionEnd - startTime).TotalDays;
            // Total days in the semester
            days[1] = (sessionEnd - sessionBegin).TotalDays;
            return days;
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