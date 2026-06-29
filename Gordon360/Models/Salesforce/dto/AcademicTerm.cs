using System;

namespace Gordon360.Models.Salesforce;


public class AcademicTerm
{
    public string gc_Jenz_Term_Code__c {get; set;} = "";
    public string gc_Jenz_Year_Code__c {get; set;} = "";
    public string Name { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    // Show on web by default
    public string ShowOnWeb { get; set; } = "B";
}
