namespace Gordon360.Services
{
    using Gordon360.Models.ViewModels;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
   

    public static class SFProfileService
    {
        private static readonly string domain = "orgfarm-7c8ce5d310-dev-ed.develop.my.salesforce.com";
        private static readonly string apiVersion = "v60.0";

        public static async Task<Dictionary<string, object>> GetAccessTokenAsync(IConfiguration config)
        {
            using var client = new HttpClient();
            var tokenUrl = $"https://{domain}/services/oauth2/token";

            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", config["Salesforce:ClientId"]),
            new KeyValuePair<string, string>("client_secret", config["Salesforce:ClientSecret"])
        });

            var response = await client.PostAsync(tokenUrl, formContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get token: {response.StatusCode}\n{content}");
            }

            return JsonSerializer.Deserialize<Dictionary<string, object>>(content);
        }

        public static async Task<ProfileViewModel> GetProfileAsync(string username, IConfiguration config)
        {
            System.Diagnostics.Debug.WriteLine("🔐 Getting Salesforce access token...");
            string clientId = config["Salesforce:ClientId"];
            string clientSecret = config["Salesforce:ClientSecret"];

            var tokenData = await GetAccessTokenAsync();
            var accessToken = tokenData["access_token"].ToString();
            var instanceUrl = tokenData["instance_url"].ToString();

            System.Diagnostics.Debug.WriteLine("🔎 Building SOQL query...");

            var soql = $@"
SELECT
    Id,
    Title__c,
    FirstName__c,
    MiddleName__c,
    LastName__c,
    Suffix__c,
    MaidenName__c,
    NickName__c,
    Email__c,
    Gender__c,
    HomeStreet1__c,
    HomeStreet2__c,
    HomeCity__c,
    HomeState__c,
    HomePostalCode__c,
    HomeCountry__c,
    HomePhone__c,
    HomeFax__c,
    AD_Username__c,
    show_pic__c,
    preferred_photo__c,
    Country__c,
    Barcode__c,
    Facebook__c,
    Twitter__c,
    Instagram__c,
    LinkedIn__c,
    Handshake__c,
    Calendar__c,

    OnOffCampus__c,
    OffCampusStreet1__c,
    OffCampusStreet2__c,
    OffCampusCity__c,
    OffCampusState__c,
    OffCampusPostalCode__c,
    OffCampusCountry__c,
    OffCampusPhone__c,
    OffCampusFax__c,
    Major3__c,
    Major3Description__c,
    Minor1__c,
    Minor1Description__c,
    Minor2__c,
    Minor2Description__c,
    Minor3__c,
    Minor3Description__c,
    GradDate__c,
    PlannedGradYear__c,
    MobilePhone__c,
    IsMobilePhonePrivate__c,
    ChapelRequired__c,
    ChapelAttended__c,
    Cohort__c,
    Class__c,
    AdvisorIDs__c,
    Married__c,
    Commuter__c,

    WebUpdate__c,
    HomeEmail__c,
    MaritalStatus__c,
    College__c,
    ClassYear__c,
    PreferredClassYear__c,
    ShareName__c,
    ShareAddress__c,

    Major__c,
    Major1Description__c,
    Major2__c,
    Major2Description__c,
    grad_student__c,

    OnCampusDepartment__c,
    Type__c,
    office_hours__c,
    Dept__c,
    Mail_Description__c,

    JobTitle__c,
    SpouseName__c,

    BuildingDescription__c,
    Mail_Location__c,
    OnCampusBuilding__c,
    OnCampusRoom__c,
    OnCampusPhone__c,
    OnCampusPrivatePhone__c,
    OnCampusFax__c,
    KeepPrivate__c,

    PersonType__c
FROM Profile__c  WHERE AD_Username__c = '{username}'
LIMIT 1";


            var queryUrl = $"{instanceUrl}/services/data/{apiVersion}/query?q={Uri.EscapeDataString(soql)}";
            System.Diagnostics.Debug.WriteLine($"🌐 Querying Salesforce: {queryUrl}");


            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(queryUrl);
            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📥 Raw response JSON: {json.Substring(0, Math.Min(json.Length, 500))}...");


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to query records: {response.StatusCode}\n{json}");
            }

            var result = JsonSerializer.Deserialize<SFQueryResponse>(json);
            if (result?.Records == null || result.Records.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ No profile found for username: " + username);
                return null;
            }

            System.Diagnostics.Debug.WriteLine("✅ Profile record found. Converting to ViewModel...");
            var record = result.Records[0];
            return ConvertToProfileViewModel(record);
        }

        private static ProfileViewModel ConvertToProfileViewModel(SFRecord record)
        {
            return new ProfileViewModel(
    record.Id,
    record.Title__c,
    record.FirstName__c,
    record.MiddleName__c,
    record.LastName__c,
    record.Suffix__c,
    record.MaidenName__c,
    record.NickName__c,
    record.Email__c,
    record.Gender__c,
    record.HomeStreet1__c,
    record.HomeStreet2__c,
    record.HomeCity__c,
    record.HomeState__c,
    record.HomePostalCode__c,
    record.HomeCountry__c,
    record.HomePhone__c,
    record.HomeFax__c,
    record.AD_Username__c,
    1,
    0,
    record.Country__c,
    record.Barcode__c,
    record.Facebook__c,
    record.Twitter__c,
    record.Instagram__c,
    record.LinkedIn__c,
    record.Handshake__c,
    record.Calendar__c,

    record.OnOffCampus__c,
    record.OffCampusStreet1__c,
    record.OffCampusStreet2__c,
    record.OffCampusCity__c,
    record.OffCampusState__c,
    record.OffCampusPostalCode__c,
    record.OffCampusCountry__c,
    record.OffCampusPhone__c,
    record.OffCampusFax__c,
    record.Major3__c,
    record.Major3Description__c,
    record.Minor1__c,
    record.Minor1Description__c,
    record.Minor2__c,
    record.Minor2Description__c,
    record.Minor3__c,
    record.Minor3Description__c,
    record.GradDate__c,
    "2027",
    "9787984031",
    true,
    record.ChapelRequired__c,
    record.ChapelAttended__c,
    record.Cohort__c,
    record.Class__c,
    record.AdvisorIDs__c,
    record.Married__c,
    record.Commuter__c,

    record.WebUpdate__c,
    record.HomeEmail__c,
    record.MaritalStatus__c,
    record.College__c,
    record.ClassYear__c,
    record.PreferredClassYear__c,
    record.ShareName__c,
    record.ShareAddress__c,

    record.Major__c,
    record.Major1Description__c,
    record.Major2__c,
    record.Major2Description__c,
    record.grad_student__c,

    record.OnCampusDepartment__c,
    record.Type__c,
    record.office_hours__c,
    record.Dept__c,
    record.Mail_Description__c,

    record.JobTitle__c,
    record.SpouseName__c,

    record.BuildingDescription__c,
    record.Mail_Location__c,
    record.OnCampusBuilding__c,
    record.OnCampusRoom__c,
    record.OnCampusPhone__c,
    record.OnCampusPrivatePhone__c,
    record.OnCampusFax__c,
    "true",

    record.PersonType__c
);

        }

        private class SFQueryResponse
        {
            [JsonPropertyName("records")]
            public List<SFRecord> Records { get; set; }
        }

        private class SFRecord
        {
            // All Profiles
            public string Id { get; set; }
            public string Title__c { get; set; }
            public string FirstName__c { get; set; }
            public string MiddleName__c { get; set; }
            public string LastName__c { get; set; }
            public string Suffix__c { get; set; }
            public string MaidenName__c { get; set; }
            public string NickName__c { get; set; }
            public string Email__c { get; set; }
            public string Gender__c { get; set; }
            public string HomeStreet1__c { get; set; }
            public string HomeStreet2__c { get; set; }
            public string HomeCity__c { get; set; }
            public string HomeState__c { get; set; }
            public string HomePostalCode__c { get; set; }
            public string HomeCountry__c { get; set; }
            public string HomePhone__c { get; set; }
            public string HomeFax__c { get; set; }
            public string AD_Username__c { get; set; }
            public object? show_pic__c { get; set; }
            public object? preferred_photo__c { get; set; }
            public string Country__c { get; set; }
            public string Barcode__c { get; set; }
            public string Facebook__c { get; set; }
            public string Twitter__c { get; set; }
            public string Instagram__c { get; set; }
            public string LinkedIn__c { get; set; }
            public string Handshake__c { get; set; }
            public string Calendar__c { get; set; }

            // Student Only
            public string OnOffCampus__c { get; set; }
            public string OffCampusStreet1__c { get; set; }
            public string OffCampusStreet2__c { get; set; }
            public string OffCampusCity__c { get; set; }
            public string OffCampusState__c { get; set; }
            public string OffCampusPostalCode__c { get; set; }
            public string OffCampusCountry__c { get; set; }
            public string OffCampusPhone__c { get; set; }
            public string OffCampusFax__c { get; set; }
            public string Major3__c { get; set; }
            public string Major3Description__c { get; set; }
            public string Minor1__c { get; set; }
            public string Minor1Description__c { get; set; }
            public string Minor2__c { get; set; }
            public string Minor2Description__c { get; set; }
            public string Minor3__c { get; set; }
            public string Minor3Description__c { get; set; }
            public string GradDate__c { get; set; }
            public object PlannedGradYear__c { get; set; }
            public object MobilePhone__c { get; set; }
            public object IsMobilePhonePrivate__c { get; set; } // string "true"/"false"
            public int? ChapelRequired__c { get; set; }
            public int? ChapelAttended__c { get; set; }
            public string Cohort__c { get; set; }
            public string Class__c { get; set; }
            public string AdvisorIDs__c { get; set; }
            public string Married__c { get; set; }
            public string Commuter__c { get; set; }

            // Alumni Only
            public string WebUpdate__c { get; set; }
            public string HomeEmail__c { get; set; }
            public string MaritalStatus__c { get; set; }
            public string College__c { get; set; }
            public string ClassYear__c { get; set; }
            public string PreferredClassYear__c { get; set; }
            public string ShareName__c { get; set; }
            public string ShareAddress__c { get; set; }

            // Student and Alumni Only
            public string Major__c { get; set; }
            public string Major1Description__c { get; set; }
            public string Major2__c { get; set; }
            public string Major2Description__c { get; set; }
            public string grad_student__c { get; set; }

            // FacStaff Only
            public string OnCampusDepartment__c { get; set; }
            public string Type__c { get; set; }
            public string office_hours__c { get; set; }
            public string Dept__c { get; set; }
            public string Mail_Description__c { get; set; }

            // FacStaff and Alumni Only
            public string JobTitle__c { get; set; }
            public string SpouseName__c { get; set; }

            // FacStaff and Student Only
            public string BuildingDescription__c { get; set; }
            public string Mail_Location__c { get; set; }
            public string OnCampusBuilding__c { get; set; }
            public string OnCampusRoom__c { get; set; }
            public string OnCampusPhone__c { get; set; }
            public string OnCampusPrivatePhone__c { get; set; }
            public string OnCampusFax__c { get; set; }
            public object KeepPrivate__c { get; set; }

            // ProfileViewModel Only
            public string PersonType__c { get; set; }
        }

    }

}
