using System;
using System.Linq;
using Gordon360.Models.ViewModels;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Gordon360.Database.CCT;

// <summary>
// We use this service to pull meal data from blackboard and parse it
// </summary>
namespace Gordon360.Services
{
    /// <summary>
    /// Service that allows for meal control
    /// </summary>
    public class DiningService : IDiningService
    {
        private readonly IConfiguration _config;

        private CCTContext _context;
        private static string issuerID;
        private static string applicationId;
        private static string secret;
        //private static string issuerID = System.Web.Configuration.WebConfigurationManager.AppSettings["bonAppetitIssuerID"];
        //private static string applicationId = System.Web.Configuration.WebConfigurationManager.AppSettings["bonAppetitApplicationID"];
        //private static string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["bonAppetitSecret"];

        public DiningService(CCTContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            issuerID = _config["bonAppetitIssuerID"];
            applicationId = _config["bonAppetitApplicationID"];
            secret = _config["bonAppetitSecret"];
        }

        private static string getTimestamp()
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan diff = DateTime.UtcNow - baseDate;
            Int64 millis = Convert.ToInt64(diff.TotalMilliseconds);
            return millis.ToString();
        }

        private static string getHash(int cardHolderID, string planID, string timestamp)
        {
            string hashstring = (secret + issuerID + cardHolderID.ToString() + planID +
            applicationId + timestamp);

            SHA1 sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(hashstring));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                // can be "x2" if you want lowercase
                sb.Append(b.ToString("x2"));
            }
            Console.WriteLine(timestamp);
            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardHolderID"></param>
        /// <param name="planID"></param>
        /// <returns></returns>
        public string GetBalance(int cardHolderID, string planID)
        {
            string balance = "";
            try
            {
                ServicePointManager.Expect100Continue = false;

                WebRequest request = WebRequest.Create("https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr");

                request.Method = "POST";

                string timestamp = getTimestamp();

                // Create POST data and convert it to a byte array.  
                string postData = "issuerId=" + issuerID + "&"
                   + "cardholderId=" + cardHolderID + "&"
                   + "planId=" + planID + "&"
                   + "applicationId=" + applicationId + "&"
                   + "valueCmd=bal" + "&" + "value=0" + "&"
                   + "timestamp=" + timestamp + "&"
                   + "hash=" + getHash(cardHolderID, planID, timestamp);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                // Get the response.  
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();

                // Read the content. 
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                JObject json = JObject.Parse(responseFromServer);
                balance = json["balance"].ToString();

                // Display the content.  
                Console.WriteLine(responseFromServer);
                Console.WriteLine("Balance: " + balance);

                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch
            {
                balance = "0";
            }

            return balance;
        }

        /// <summary>
        /// Get information about the selected plan for the student user
        /// </summary>
        /// <param name="cardHolderID">Student's Gordon ID</param>
        /// <param name="sessionCode">Current Session Code</param>
        /// <returns></returns>
        public DiningViewModel GetDiningPlanInfo(int cardHolderID, string sessionCode)
        {
            var result = _context.DiningInfo.Where(d => d.StudentId == cardHolderID && d.SessionCode == sessionCode)
                .Select(d => new DiningTableViewModel
                { 
                    ChoiceDescription = d.ChoiceDescription,
                    PlanDescriptions = d.PlanDescriptions,
                    PlanId = d.PlanId,
                    PlanType = d.PlanType,
                    InitialBalance = d.InitialBalance ?? 0 
                });

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The plan was not found." };
            }

            foreach (var row in result)
            {
                row.CurrentBalance = GetBalance(cardHolderID, row.PlanId);
            }

            return new DiningViewModel(result);
        }
    }
}
