using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Gordon360.Options;

// <summary>
// We use this service to pull meal data from blackboard and parse it
// </summary>
namespace Gordon360.Services;

/// <summary>
/// Service that allows for meal control
/// </summary>
public class DiningService(CCTContext context, IOptions<BonAppetitOptions> options) : IDiningService
{
    private BonAppetitOptions Options = options.Value;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardHolderID"></param>
    /// <param name="planID"></param>
    /// <returns></returns>
    public async Task<string> GetBalanceAsync(int cardHolderID, string planID)
    {
        try
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            HttpRequestMessage request = new(HttpMethod.Post, "https://bbapi.campuscardcenter.com/cs/api/mealplanDrCr")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    ["issuerId"] = Options.IssuerID.ToString(),
                    ["cardholderId"] = cardHolderID.ToString(),
                    ["planId"] = planID,
                    ["applicationId"] = Options.ApplicationID,
                    ["valueCmd"] = "bal",
                    ["value"] = "0",
                    ["timestamp"] = timestamp.ToString(),
                    ["hash"] = GetHash(cardHolderID, planID, timestamp.ToString()),
                })
            };

            using var client = new HttpClient();
            var response = await client.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();
            JsonNode? json = JsonNode.Parse(responseString);
            string? balance = json?["balance"]?.GetValue<string>();

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
    public async Task<DiningViewModel> GetDiningPlanInfoAsync(int cardHolderID, string sessionCode)
    {
        List<DiningTableViewModel> result = [];
        foreach (var plan in context.DiningInfo.Where(d => d.StudentId == cardHolderID && d.SessionCode == sessionCode))
        {
            result.Add(new DiningTableViewModel
            {
                ChoiceDescription = plan.ChoiceDescription,
                PlanDescriptions = plan.PlanDescriptions,
                PlanId = plan.PlanId,
                PlanType = plan.PlanType,
                InitialBalance = plan.InitialBalance ?? 0,
                CurrentBalance = await GetBalanceAsync(cardHolderID, plan.PlanId)
            });
        }

        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The plan was not found." };
        }

        return new DiningViewModel(result);
    }

    private string GetHash(int cardHolderID, string planID, string timestamp)
    {
        string hashstring = Options.Secret
                            + Options.IssuerID
                            + cardHolderID.ToString()
                            + planID
                            + Options.ApplicationID
                            + timestamp;
        byte[] hash = SHA1.HashData(Encoding.ASCII.GetBytes(hashstring));
        return Convert.ToHexString(hash);
    }
}
