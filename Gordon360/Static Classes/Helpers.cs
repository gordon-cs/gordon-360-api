using Gordon360.Models.CCT.Context;
using System;
using System.Linq;

namespace Gordon360.Static.Methods
{
    /// <summary>
    /// Service class for methods that are shared between all services.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Helper method that gets the current session we are in.
        /// </summary>
        /// <returns>The session code of the current session</returns>
        public static string GetCurrentSession(CCTContext context)
        {
            return context.CM_SESSION_MSTR.Where(s => DateTime.Now > s.SESS_BEGN_DTE).OrderByDescending(s => s.SESS_BEGN_DTE).Select(s => s.SESS_CDE).First();
        }

        /// <summary>
        /// Helper method that casts given date time to type UTC without changing the time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>dateTime with type UTC, if dateTime is null, will return DateTime min value</returns>
        public static DateTime FormatDateTimeToUtc(DateTime? dateTime)
        {
            if (dateTime is null) return DateTime.MinValue;
            return DateTime.SpecifyKind((DateTime)dateTime, DateTimeKind.Utc);
        }
    }
}
