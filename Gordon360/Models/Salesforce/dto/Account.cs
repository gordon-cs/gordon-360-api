using System;

namespace Gordon360.Models.Salesforce;



public class Account
{
    public string Name { get; set; } = "";
    public string gc_University_Email__c { get; set; } = "";
    
    public SFChildCollections<ConstituentRole> Persons { get; set; } = new();
    public SFChildCollections<LearnerProgram> LearnerPrograms { get; set; } = new();
    public SFChildCollections<ContactPointAddress> ContactPointAddresses { get; set; } = new();
    public SFChildCollections<Contact> Contacts { get; set; } = new();
}
