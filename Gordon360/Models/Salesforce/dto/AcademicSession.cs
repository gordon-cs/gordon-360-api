namespace Gordon360.Models.Salesforce;

public class AcademicSession
{
    public string gc_Jenz_Session_Code__c { get; set; } = "";
    public string gc_Jenz_Subterm_Code__c { get; set; } = "";
    public string gc_Jenz_Term_Code__c { get; set; } = "";
    public string gc_Jenz_Year_Code__c { get; set; } = "";

    public AcademicTerm AcademicTerm { get; set; } = new();

}