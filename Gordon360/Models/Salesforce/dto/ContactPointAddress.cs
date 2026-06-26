namespace Gordon360.Models.Salesforce;


public class ContactPointAddress
{
    public string Name { get; set; } = "";
    public string gc_Status__c { get; set; } = "";

    public AcademicTerm Academic_Term__r { get; set; } = new();
    public Location gc_On_Campus_Location__r { get; set; } = new();

}
