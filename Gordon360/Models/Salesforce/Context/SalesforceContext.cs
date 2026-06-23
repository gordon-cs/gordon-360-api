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
namespace Gordon360.Models.Salesforce.Context;

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
    
    private string GetSalesforceObjectName<T>()
    {
        var attribute = typeof(T).GetCustomAttributes<SalesforceObjectAttribute>().FirstOrDefault();
        if (attribute == null)
        {
            throw new Exception($"Type {typeof(T).Name} does not have a SalesforceObjectAttribute.");
        }
        return attribute.Name;
    }

    // get list of json deserialize field names
    private List<string> GetSalesforceFieldNames<T>()
    {
        var properties = typeof(T).GetProperties();
        var fieldNames = new List<string>();
        foreach (var prop in properties)
        {
            var jsonAttr = prop.GetCustomAttributes<JsonPropertyNameAttribute>().FirstOrDefault();
            if (jsonAttr != null)
            {
                fieldNames.Add(jsonAttr.Name);
            }
        }
        return fieldNames;
    }

    /// <summary>
    /// Generic method to query any salesforce object and return a list of view models
    /// </summary>
    /// <typeparam name="T">The type of view model to return, must have SalesforceObject and JsonPropertyName attributes</typeparam>
    /// <param name="queryParameter">The WHERE clause or other SOQL parameters to filter the query, e.g. "WHERE StudentId__c = '12345'"</param>
    /// <returns>A list of view models of type T</returns>
    public async Task<List<T>> Query<T>(string queryParameter)
    {

        var objectName = GetSalesforceObjectName<T>();
        // put proper error handling here if the name is not found
            
        System.Diagnostics.Debug.WriteLine("🔐 Getting Salesforce access token...");
        var tokenData = await GetAccessTokenAsync(config);
        var accessToken = tokenData["access_token"].ToString();
        var instanceUrl = tokenData["instance_url"].ToString();

        System.Diagnostics.Debug.WriteLine($"access token: {accessToken}");


        var soql = $@"
            SELECT
                {string.Join(", ", GetSalesforceFieldNames<T>())}
            FROM {objectName} 
            {queryParameter}";

        System.Diagnostics.Debug.WriteLine(soql);

        var queryUrl = $"{instanceUrl}/services/data/{ApiVersion}/query?q={Uri.EscapeDataString(soql)}";
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


        return results?.records;
    }

    public async Task<string> QueryJson(string queryString)
    {

        // put proper error handling here if the name is not found
            
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

        return  json;
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
