using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

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
        private readonly CCTContext _context;
        private static BonAppetitSettings settings;

        public DiningService(CCTContext context, IConfiguration config)
        {
            _context = context;
             settings = config.GetSection(BonAppetitSettings.BonAppetit).Get<BonAppetitSettings>() ?? throw new BadInputException{ ExceptionMessage = "Failed to load Dining API Settings"};
        }

        private static string GetHash(int cardHolderID, string planID, string timestamp)
        {
            string hashstring = (
                settings.Secret
                + settings.IssuerID
                + cardHolderID.ToString()
                + planID
                + settings.ApplicationID
                + timestamp);

            using var sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashstring));
            return Convert.ToHexString(hash);
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
        public static string GetBalance(int cardHolderID, string planID)
        {
            try
            {

                ServicePointManager.Expect100Continue = false;

                WebRequest request = WebRequest.Create("https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr");

                request.Method = "POST";

                string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                // Create POST data and convert it to a byte array.  
                string postData = $"issuerId={settings.IssuerID}&cardholderId={cardHolderID}&planId={planID}&applicationId={settings.ApplicationID}&valueCmd=bal&value=0&timestamp={timestamp}&hash={getHash(cardHolderID, planID, timestamp)}";
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
                string balance = json["balance"].ToString();

                // Display the content.  
                Console.WriteLine(responseFromServer);
                Console.WriteLine("Balance: " + balance);

                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
                return balance;
            }
            catch
            {
                return "0";
            }
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
                    InitialBalance = d.InitialBalance ?? 0,
                    CurrentBalance = GetBalance(cardHolderID, d.PlanId)
                });

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The plan was not found." };
            }

            return new DiningViewModel(result);
        }

        public class BonAppetitSettings
        {
            public const string BonAppetit = "BonAppetit";

            public string IssuerID {get; set;} = String.Empty;
            public string ApplicationID {get; set;} = String.Empty;
            public string Secret {get; set;} = String.Empty;
        }
    }
}
