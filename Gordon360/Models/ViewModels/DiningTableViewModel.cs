using System.Text.Json.Serialization;
using Gordon360.Models.Salesforce;

namespace Gordon360.Models.ViewModels;

[SalesforceObject(SFDiningInfo.SFObjectName)]
public class DiningTableViewModel
{
    [JsonPropertyName(SFDiningInfo.ChoiceDescription)]
    public string ChoiceDescription { get; set; }
    
    [JsonPropertyName(SFDiningInfo.PlanDescriptions)]
    public string PlanDescriptions { get; set; }
    
    [JsonPropertyName(SFDiningInfo.PlanId)]
    public string PlanId { get; set; }
    
    [JsonPropertyName(SFDiningInfo.PlanType)]
    public string PlanType { get; set; }

    [JsonPropertyName(SFDiningInfo.InitialBalance)]
    public decimal InitialBalance { get; set; }

    [JsonPropertyName(SFDiningInfo.CurrentBalance)]
    public string CurrentBalance { get; set; }
}