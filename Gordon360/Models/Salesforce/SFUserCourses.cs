namespace Gordon360.Models.Salesforce;

using System.Text.Json.Serialization;
using Gordon360.Models.Salesforce.Attributes;

[SalesforceObject("Contact")]
public class SFAccount
{
    public static class FieldNames
    {
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string Email = "Email";
        public const string Phone = "Phone";
    }

    [JsonPropertyName(FieldNames.FirstName)]
    public string FirstName { get; set; }

    [JsonPropertyName(FieldNames.LastName)]
    public string LastName { get; set; }

    [JsonPropertyName(FieldNames.Email)]
    public string Email { get; set; }

    [JsonPropertyName(FieldNames.Phone)]
    public string Phone { get; set; }

}

