namespace Gordon360.Models.Salesforce;


public class Contact
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";

    public string gc_Preferred_Class__c { get; set; } = "";
    public string MaritalStatus { get; set; } = "";

    public string gc_Current_Positions__c { get; set; } = ""; // comma-separated list of current positions

    public SFChildCollection<ContactContactRelation> CCRContacts { get; set; } = new();
}
