using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gordon360.Models.Salesforce;

public class SalesforceContext : ISalesforceContext
{
    public IConfiguration config;
    private static string ClientId;
    private static string ClientSecret;
    private static string OrganizationUrl;
    private static string ApiVersion;


    public SalesforceContext(IConfiguration config)
    {
        this.config = config;
        var sf = "SalesforceStandard";

        ClientId = config[$"{sf}:ClientId"];
        ClientSecret = config[$"{sf}:ClientSecret"];
        OrganizationUrl = config[$"{sf}:OrganizationUrl"];
        ApiVersion = config[$"{sf}:ApiVersion"];
    }


    public async Task<SFQueryResult<T>> RawQuery<T>(string queryString) // QueryType queryType = QueryType.SOQL)
    {            
        var queryType = QueryType.SOQL;
        System.Diagnostics.Debug.WriteLine("🔐 Getting Salesforce access token...");
        var tokenData = await GetAccessTokenAsync(config);
        var accessToken = tokenData["access_token"].ToString();
        var instanceUrl = tokenData["instance_url"].ToString();
        var requestString = queryType == QueryType.SOQL ? "query" : "search";

        System.Diagnostics.Debug.WriteLine($"access token: {accessToken}");

        System.Diagnostics.Debug.WriteLine(queryString);

        var queryUrl = $"{instanceUrl}/services/data/{ApiVersion}/{requestString}?q={Uri.EscapeDataString(queryString)}";
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


    static async Task<Dictionary<string, object>> GetAccessTokenAsync(IConfiguration config)
    {
        using var client = new HttpClient();
        var tokenUrl = $"https://{OrganizationUrl}/services/oauth2/token";

        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("client_secret", ClientSecret)
        ]);

        var response = await client.PostAsync(tokenUrl, formContent);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get token: {response.StatusCode}\n{content}");
        }

        return JsonSerializer.Deserialize<Dictionary<string, object>>(content);
    }

    /// <summary>
    /// Constructs a SOQL query
    /// </summary>
    /// <param name="template">Template string in the form 'SELECT [fields] FROM [table]'</param>
    /// <param name="where">SOQL field selectors</param>
    /// <param name="order">SOQL ordering</param>
    /// <param name="limit_n">SOQL limit on number of records returned</param>
    /// <returns>T containing results of query</returns>
    public async Task<SFQueryResult<T>> SoqlQuery<T>(string template, string? where = null, string? order = null, int limit_n = 0)
    {
        // Prepare strings for injection
        template += " {0} {1} {2}";
        where = string.IsNullOrEmpty(where) ? "" : ("WHERE " + where);
        order = string.IsNullOrEmpty(order) ? "" : ("ORDER BY " + order);
        string limit = limit_n == 0 ? "" : ("LIMIT " + limit_n);

        string query = string.Format(template, where, order, limit);
        var response = await RawQuery<T>(query);
        return response;
    }

}
