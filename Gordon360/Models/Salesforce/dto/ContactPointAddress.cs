namespace Gordon360.Models.Salesforce;


public class ContactPointAddress
{
    public string Name { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string gc_Status__c { get; set; } = "";
    public string AddressType { get; set; } = "";
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
    public string CountryCode { get; set; } = "";



    public AcademicTerm Academic_Term__r { get; set; } = new();
    public Location gc_On_Campus_Location__r { get; set; } = new();

}
