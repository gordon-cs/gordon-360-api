namespace Gordon360.Models.Salesforce;


public class Location
{
    public string ExternalReference { get; set; } = "";

    public string Name { get; set; } = "";
    public Location? ParentLocation { get; set; } = null;
    public string gc_Jenz_Building_Code__c { get; set; } = "";
    public string Phone { get; set; } = "";
    public string gc_Jenz_Room_Code__c { get; set; } = "";
}
