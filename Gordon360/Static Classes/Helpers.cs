using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Static.Methods
{
    /// <summary>
    /// Service class for methods that are shared between all services.
    /// </summary>
    public static class Helpers
    {
        private static CCTContext Context => new();

        /// <summary>
        /// Service method that gets the current session we are in.
        /// </summary>
        /// <returns>SessionViewModel of the current session. If no session is found for our current date, returns null.</returns>
        public static async Task<SessionViewModel> GetCurrentSessionAsync()
        {
            // TODO: Pass CCTEntities context by configuration/options from startup
            var sessionService = new SessionService(Context);

            var query = await Context.Procedures.CURRENT_SESSIONAsync();
            var currentSessionCode = query.Select(x => x.DEFAULT_SESS_CDE).FirstOrDefault();

            return sessionService.Get(currentSessionCode);
        }

        // Return the first day in the current session
        public static async Task<string> GetFirstDayAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            DateTime firstDayRaw = currentSession.SessionBeginDate.Value;
            string firstDay = firstDayRaw.ToString("MM/dd/yyyy");
            return firstDay;
        }

        // Return the last day in the current session
        public static async Task<string> GetLastDayAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            DateTime lastDayRaw = currentSession.SessionEndDate.Value;
            string lastDay = lastDayRaw.ToString("MM/dd/yyyy");
            return lastDay;
        }

        // Return the days left in the semester, and the total days in the current session
        public static async Task<double[]> GetDaysLeftAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
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

        public static string GetLeaderRoleCodes()
        {
            return "LEAD";
        }

        public static string GetAdvisorRoleCodes()
        {
            return "ADV";
        }

        public static string GetTranscriptWorthyRoles()
        {
            //string[] transcriptWorthyRoles = { "CAPT", "CODIR", "CORD", "DIREC", "PRES", "VICEC", "VICEP", "AC", "RA1", "RA2","RA3", "SEC" };
            return "LEAD";
        }
    }
}
