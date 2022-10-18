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
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private static HttpClient HttpClient => new();

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
        public static async Task<string> GetBalanceAsync(int cardHolderID, string planID)
        {
            try
            {

                ServicePointManager.Expect100Continue = false;

                WebRequest request = WebRequest.Create("https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr");

                request.Method = "POST";

                string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                var data = new Dictionary<string, string>
                {
                    { "issuerId", settings.IssuerID },
                    { "cardholderId", cardHolderID.ToString() },
                    {"planId", planID.ToString() },
                    {"applicationId", settings.ApplicationID },
                    { "valueCmd", "bal" },
                    {"value", "0" },
                    {"timestamp", timestamp},
                    {"hash", GetHash(cardHolderID, planID, timestamp) }
                };

                FormUrlEncodedContent encodedData = new(data);

                var response = await HttpClient.PostAsync("https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr", encodedData);
                var content = await response.Content.ReadAsStringAsync();
                JObject jsonContent = JObject.Parse(content);

                if (jsonContent["balance"]?.ToString() is string balance)
                {
                return balance;
            }
                else
                {
                    return "0";
                }
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
        public async Task<DiningViewModel> GetDiningPlanInfo(int cardHolderID, string sessionCode)
        {
            var result = _context.DiningInfo.Where(d => d.StudentId == cardHolderID && d.SessionCode == sessionCode);

            var planComponents = new List<DiningTableViewModel>();
            foreach (var diningPlan in result)
                {
                var currentBalance = await GetBalanceAsync(cardHolderID, diningPlan.PlanId);
                planComponents.Add(new DiningTableViewModel
                {
                    ChoiceDescription = diningPlan.ChoiceDescription,
                    PlanDescriptions = diningPlan.PlanDescriptions,
                    PlanId = diningPlan.PlanId,
                    PlanType = diningPlan.PlanType,
                    InitialBalance = diningPlan.InitialBalance ?? 0,
                    CurrentBalance = currentBalance,
                });

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The plan was not found." };
            }

            return new DiningViewModel(planComponents);
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
