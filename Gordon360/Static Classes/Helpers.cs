using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Gordon360.Services.ComplexQueries;
using Gordon360.Services;
using Gordon360.Models;
using System.Collections.Generic;

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

        // Return the days left in the semester, and the total days in the semester
        public static double[] GetDaysLeft()
        {
            // The end of the current session
            DateTime sessionEnd = GetCurrentSession().SessionEndDate.Value;
            DateTime sessionBegin = GetCurrentSession().SessionBeginDate.Value;
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
        /// Return an XDocument from a URL containing XML. 
        /// This is used to retrieve data from 25Live specifically.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static XDocument GetLiveStream(string requestUrl)
        {

            // Send the request and parse 
            using (WebClient client = new WebClient())
            {
                MemoryStream stream = null;
                // Commit contents of the request to temporary memory
                try
                {
                    Uri request = new Uri(requestUrl);
                    // Use an Async method to make sure we have completed the download 
                    // We don't want to try and pull partial data!
                    var data = client.DownloadData(request);
                    stream = new MemoryStream(data);
                }
                // catch any errors thrown
                catch (ArgumentNullException e)
                {
                    // The DownloadData function didn't return anything!
                }
                catch (WebException e)
                {
                    // The DownloadData function wasn't able to connect to the source!
                }

                // Begin to read contents with correct encoding
                XDocument xmlDoc = null;
                try
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        // Create a string of content
                        string content = reader.ReadToEnd();
                        // Load the data into an XmlDocument
                        xmlDoc = XDocument.Parse(content);

                    }
                }
                catch (ArgumentException e)
                {
                    // Something was wrong with the memory stream
                }

                return xmlDoc;
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

        // Fill an iterable list of basicinfo from a query to the database
        public static IEnumerable<BasicInfoViewModel> GetAllBasicInfo()
        {
            // Create a list to be filled
            IEnumerable<BasicInfoViewModel> result = null;
            try
            {
                // Attempt to query the DB
                result = RawSqlQuery<BasicInfoViewModel>.query("ALL_BASIC_INFO");
            }
            catch
            {
                //
            }
            // Filter out results with null or empty active directory names
            return result;
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