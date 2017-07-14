using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Gordon360.Services.ComplexQueries;
using Gordon360.Services;

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
            var sessionService = new SessionService(tempUnitOfWork);

            var query = RawSqlQuery<String>.query("CURRENT_SESSION");
            var currentSessionCode = query.Select(x => x).FirstOrDefault();

            SessionViewModel result = sessionService.Get(currentSessionCode);

            return result; ;
        }

        // Return a memorystream from a specific URL
        public static MemoryStream GetLiveStream(string requestUrl)
        {

            // Send the request and parse 
            using (WebClient client = new WebClient())
            {
                // Commit contents of the request to temporary memory
                MemoryStream stream = new MemoryStream(client.DownloadData(requestUrl));
                // Begin to read contents with correct encoding
                return stream;
            }
        }

        /// <summary>
        ///  Helper function to determine the current academic year
        /// </summary>
        /// <returns></returns>
        public static string GetDay()
        {
            // We need to determine what the current date is
            DateTime today = DateTime.Today;
            if (today.Month < 06)
            {
                return (today.Year - 1).ToString();
            }
            else if (today.Month > 07)
            {
                return (today.Year).ToString();
            }
            return today.Year.ToString();
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