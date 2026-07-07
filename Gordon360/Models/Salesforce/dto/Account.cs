using System;

namespace Gordon360.Models.Salesforce;



public class Account
{
    public string Name { get; set; } = "";
    public string PersonEmail { get; set; } = "";

    public string Student_Id__pc { get; set; } = "";
    public string gc_University_Email__c { get; set; } = "";
    public string PersonTitle { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Suffix__pc { get; set; } = "";
    public string FormerLastName__pc { get; set; } = ""; // maiden name?
    public string Preferred_First_Name_Formula__pc { get; set; } = ""; // maiden name?
    public string AD_Username__pc { get; set; } = "";
    public string PersonGenderIdentity { get; set; } = "";


    public SFChildCollection<ConstituentRole> Persons { get; set; } = new();
    public SFChildCollection<LearnerProgram> LearnerPrograms { get; set; } = new();
    public SFChildCollection<ContactPointAddress> ContactPointAddresses { get; set; } = new();
    public SFChildCollection<Contact?>? Contacts { get; set; } = null;
    public SFChildCollection<PersonEmployment> PersonEmployments { get; set; } = new();
}
