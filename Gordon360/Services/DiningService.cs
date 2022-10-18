using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

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
        private const string DiningAPIUrl = "https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr";
        private readonly CCTContext _context;
        private static BonAppetitSettings settings;
        private static HttpClient HttpClient => new();

        public DiningService(CCTContext context, IConfiguration config)
        {
            _context = context;
            settings = config.GetSection(BonAppetitSettings.BonAppetit).Get<BonAppetitSettings>() ?? throw new BadInputException { ExceptionMessage = "Failed to load Dining API Settings" };
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardHolderID"></param>
        /// <param name="planID"></param>
        /// <returns></returns>
        public static async Task<string> GetBalanceAsync(int cardHolderID, string planID)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("issuerId", settings.IssuerID),
                new KeyValuePair<string, string>("cardholderId", cardHolderID.ToString()),
                new KeyValuePair<string, string>("planId", planID.ToString()),
                new KeyValuePair<string, string>("applicationId", settings.ApplicationID),
                new KeyValuePair<string, string>("valueCmd", "bal"),
                new KeyValuePair<string, string>("value", "0"),
                new KeyValuePair<string, string>("timestamp", timestamp),
                new KeyValuePair<string, string>("hash", GetHash(cardHolderID, planID, timestamp)),
            };

            try
            {
                var response = await HttpClient.PostAsync(DiningAPIUrl, new FormUrlEncodedContent(requestParams));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var balance = json.RootElement.GetProperty("balance").GetString();
                return balance ?? "0";
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
            foreach (var diningInfo in result)
            {
                var currentBalance = await GetBalanceAsync(cardHolderID, diningInfo.PlanId);
                planComponents.Add(new DiningTableViewModel
                {
                    ChoiceDescription = diningInfo.ChoiceDescription,
                    PlanDescriptions = diningInfo.PlanDescriptions,
                    PlanId = diningInfo.PlanId,
                    PlanType = diningInfo.PlanType,
                    InitialBalance = diningInfo.InitialBalance ?? 0,
                    CurrentBalance = currentBalance,
                });
            }

            return new DiningViewModel(planComponents);
        }

        public class BonAppetitSettings
        {
            public const string BonAppetit = "BonAppetit";

            public string IssuerID { get; set; } = string.Empty;
            public string ApplicationID { get; set; } = string.Empty;
            public string Secret { get; set; } = string.Empty;
        }
    }
}
