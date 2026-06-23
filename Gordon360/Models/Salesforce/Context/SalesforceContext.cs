using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.Salesforce;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gordon360.Models.Salesforce.Attributes;
using System.Reflection;

// <summary>
// We use this service to pull meal data from blackboard and parse it
// </summary>
namespace Gordon360.Models.Salesforce;

/// <summary>
/// Service that allows for meal control
/// </summary>
public class SalesforceContext
{
    public IConfiguration config;
    private static string ClientId;
    private static string ClientSecret;
    private static string OrganizationUrl;
    private static string ApiVersion;


    public SalesforceContext(IConfiguration config)
    {
        this.config = config;
        var sf = "Salesforce";

        ClientId = config[$"{sf}:ClientId"];
        ClientSecret = config[$"{sf}:ClientSecret"];
        OrganizationUrl = config[$"{sf}:OrganizationUrl"];
        ApiVersion = config[$"{sf}:ApiVersion"];
    }
    

    public async Task<SFQueryResult<T>> Query<T>(string queryString)
    {            
        System.Diagnostics.Debug.WriteLine("🔐 Getting Salesforce access token...");
        var tokenData = await GetAccessTokenAsync(config);
        var accessToken = tokenData["access_token"].ToString();
        var instanceUrl = tokenData["instance_url"].ToString();

        System.Diagnostics.Debug.WriteLine($"access token: {accessToken}");

        System.Diagnostics.Debug.WriteLine(queryString);

        var queryUrl = $"{instanceUrl}/services/data/{ApiVersion}/query?q={Uri.EscapeDataString(queryString)}";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync(queryUrl);
        var json = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"📥 Raw response JSON: {json}...");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to query records: {response.StatusCode}\n{json}");
        }

        var results = JsonSerializer.Deserialize<SFQueryResult<T>>(json);


        return results;
    }


    public static async Task<Dictionary<string, object>> GetAccessTokenAsync(IConfiguration config)
    {
        using var client = new HttpClient();
        var tokenUrl = $"https://{OrganizationUrl}/services/oauth2/token";

        var formContent = new FormUrlEncodedContent(new[]{
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("client_secret", ClientSecret)
        });

        var response = await client.PostAsync(tokenUrl, formContent);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get token: {response.StatusCode}\n{content}");
        }

        return JsonSerializer.Deserialize<Dictionary<string, object>>(content);
    }


}
