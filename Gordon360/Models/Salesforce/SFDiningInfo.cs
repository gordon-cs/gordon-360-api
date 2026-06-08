namespace Gordon360.Models.Salesforce;

using System.Text.Json.Serialization;
using Gordon360.Models.Salesforce.Attributes;

[SalesforceObject("DiningInfo__c")]
public class SFDiningInfo
{
    public static class FieldNames
    {
        public const string StudentId = "StudentId__c";
        public const string SessionCode = "SessionCode__c";
        public const string ChoiceDescription = "Choice_Description__c";
        public const string PlanDescriptions = "PlanDescriptions__c";
        public const string PlanId = "PlanId__c";
        public const string PlanType = "PlanType__c";
        public const string InitialBalance = "InitialBalance__c";
    }

    [JsonPropertyName(FieldNames.StudentId)]
    public decimal StudentId { get; set; }

    [JsonPropertyName(FieldNames.SessionCode)]
    public string SessionCode { get; set; }

    [JsonPropertyName(FieldNames.ChoiceDescription)]
    public string ChoiceDescription { get; set; }

    [JsonPropertyName(FieldNames.PlanDescriptions)]
    public string PlanDescriptions { get; set; }

    [JsonPropertyName(FieldNames.PlanId)]
    public string PlanId { get; set; }

    [JsonPropertyName(FieldNames.PlanType)]
    public string PlanType { get; set; }

    [JsonPropertyName(FieldNames.InitialBalance)]
    public decimal? InitialBalance { get; set; }

}