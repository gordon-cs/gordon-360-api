using System.Text.Json.Serialization;
using Gordon360.Models.Salesforce;
using Gordon360.Models.Salesforce.Attributes;

namespace Gordon360.Models.ViewModels;

public class DiningTableViewModel
{
    public string ChoiceDescription { get; set; }
    
    public string PlanDescriptions { get; set; }
    
    public string PlanId { get; set; }
    
    public string PlanType { get; set; }

    public decimal InitialBalance { get; set; }

    public string CurrentBalance { get; set; }

}