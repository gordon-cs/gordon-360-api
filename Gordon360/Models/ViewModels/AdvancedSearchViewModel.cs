using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public class AdvancedSearchViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NickName { get; set; }
    public string MaidenName { get; set; }
    public string HomeCity { get; set; }
    public string HomeState { get; set; }
    public string Country { get; set; }
    public string Email { get; set; }
    public string AD_Username { get; set; }
    public string Hall { get; set; }
    public string Class { get; set; }
    public string Major1Description { get; set; }
    public string Major2Description { get; set; }
    public string Major3Description { get; set; }
    public string Minor1Description { get; set; }
    public string Minor2Description { get; set; }
    public string Minor3Description { get; set; }
    public string KeepPrivate { get; set; }
    public string Mail_Location { get; set; }
    public string OnCampusDepartment { get; set; }
    public string BuildingDescription { get; set; }
    public string JobTitle { get; set; }
    public string Type { get; set; }
    public string ShareName { get; set; }
    public string PreferredClassYear { get; set; }
    public string ShareAddress { get; set; }
    public string Gender { get; set; }

    public static implicit operator AdvancedSearchViewModel(Student s)
    {
        return new AdvancedSearchViewModel
        {
            FirstName = s.FirstName ?? "",
            LastName = s.LastName ?? "",
            NickName = s.NickName ?? "",
            MaidenName = s.MaidenName ?? "",
            HomeCity = s.HomeCity ?? "",
            HomeState = s.HomeState ?? "",
            Country = s.Country ?? "",
            Email = s.Email ?? "",
            AD_Username = s.AD_Username ?? "",
            Hall = s.BuildingDescription ?? "",
            Class = s.Class ?? "",
            Major1Description = s.Major1Description ?? "",
            Major2Description = s.Major2Description ?? "",
            Major3Description = s.Major3Description ?? "",
            Minor1Description = s.Minor1Description ?? "",
            Minor2Description = s.Minor2Description ?? "",
            Minor3Description = s.Minor3Description ?? "",
            KeepPrivate = s.KeepPrivate ?? "",
            Mail_Location = s.Mail_Location ?? "",
            OnCampusDepartment = "",
            BuildingDescription = s.BuildingDescription ?? "",
            JobTitle = "",
            Type = "Student",
            ShareName = "",
            PreferredClassYear = "",
            ShareAddress = "",
            Gender = s.Gender ?? ""
        };
    }

    public static implicit operator AdvancedSearchViewModel(FacStaff fs)
    {
        return new AdvancedSearchViewModel
        {
            FirstName = fs.FirstName ?? "",
            LastName = fs.LastName ?? "",
            NickName = fs.Nickname ?? "",
            MaidenName = fs.MaidenName ?? "",
            HomeCity = fs.HomeCity ?? "",
            HomeState = fs.HomeState ?? "",
            Country = fs.Country ?? "",
            Email = fs.Email ?? "",
            AD_Username = fs.AD_Username ?? "",
            Hall = fs.BuildingDescription ?? "",
            Class = "",
            Major1Description = "",
            Major2Description = "",
            Major3Description = "",
            Minor1Description = "",
            Minor2Description = "",
            Minor3Description = "",
            KeepPrivate = fs.KeepPrivate ?? "",
            Mail_Location = fs.Mail_Location ?? "",
            OnCampusDepartment = fs.OnCampusDepartment ?? "",
            BuildingDescription = fs.BuildingDescription ?? "",
            JobTitle = fs.JobTitle ?? "",
            Type = fs.Type ?? "",
            ShareName = "",
            PreferredClassYear = "",
            ShareAddress = "",
            Gender = fs.Gender ?? ""
        };
    }

    public static implicit operator AdvancedSearchViewModel(Alumni a)
    {
        return new AdvancedSearchViewModel
        {
            FirstName = a.FirstName ?? "",
            LastName = a.LastName ?? "",
            NickName = a.NickName ?? "",
            MaidenName = a.MaidenName ?? "",
            HomeCity = a.HomeCity ?? "",
            HomeState = a.HomeState ?? "",
            Country = a.Country ?? "",
            Email = a.Email ?? "",
            AD_Username = a.AD_Username ?? "",
            Hall = "",
            Class = "",
            Major1Description = a.Major1Description ?? "",
            Major2Description = a.Major2Description ?? "",
            Major3Description = "",
            Minor1Description = "",
            Minor2Description = "",
            Minor3Description = "",
            KeepPrivate = "",
            Mail_Location = "",
            OnCampusDepartment = "",
            BuildingDescription = "",
            JobTitle = "",
            Type = "Alumni",
            ShareName = a.ShareName ?? "",
            PreferredClassYear = a.PreferredClassYear ?? "",
            ShareAddress = a.ShareAddress ?? "",
            Gender = a.Gender ?? ""
        };
    }

    public static explicit operator AdvancedSearchViewModel(Account account)
    {
        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();

        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home")
                            ?? new ContactPointAddress();

        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();

        string personType = (account.gc_Current_Student__c ? "stu" : "") + (account.gc_is_Current_Alumni__c ? "alu" : "") + (account.gc_Current_Faculty__pc ? "fac" : "");

        var address =
            account.ContactPointAddresses?.records?.FirstOrDefault();

        var advisorsIds = string.Join(",",
           account.Contacts?.records?
               .SelectMany(c => c?.CCRContacts?.records ?? [])
               .Where(c => c.PartyRoleRelation?.Name?.Contains("Advisor") == true)
               .Select(c => c.RelatedContact.Name)
               .Where(name => !string.IsNullOrEmpty(name))
           ?? []);

        var majors = account.LearnerPrograms?.records?
                         .Where(x => x.LearningProgramPlan?.LearningProgram?.Type__c == "Major")
                         .Take(3)
                         .ToList() ?? [];

        var minors = account.LearnerPrograms?.records?
                         .Where(x => x.LearningProgramPlan?.LearningProgram?.Type__c == "Minor")
                         .Take(3)
                         .ToList()
                     ?? [];

        var employment = account.PersonEmployments?.records?.FirstOrDefault() ?? new PersonEmployment();

        return new AdvancedSearchViewModel
        {
            FirstName = account.FirstName ?? "",
            LastName = account.LastName ?? "",
            NickName = account.Preferred_First_Name_Formula__pc ?? "",
            MaidenName = account.FormerLastName__pc ?? "",
            HomeCity = homeAddress.City ?? "",
            HomeState = homeAddress.State ?? "",
            Country = homeAddress.Country ?? "",
            Email = account.PersonEmail ?? "",
            AD_Username = account.AD_Username__pc ?? "",
            Hall = onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            Class = "", // TODO: Implement Class
            Major1Description = majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major2Description = majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major3Description = majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor1Description = minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor2Description = minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor3Description = minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            KeepPrivate = "", // TODO: implement KeepPrivate
            Mail_Location = "", // TODO: implement Mail_Location
            OnCampusDepartment = "", // TODO: Implement OnCampusDepartment
            BuildingDescription = onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            JobTitle = employment?.Position ?? "",
            Type = account.gc_Current_Student__c ? "Student"
                : account.gc_Current_Staff__pc ? "Staff"
                : account.gc_is_Current_Alumni__c ? "Alumni"
                : account.gc_Current_Faculty__pc ? "Faculty" : "",
            ShareName = "", // TODO: Implement ShareName
            PreferredClassYear = contact.gc_Preferred_Class__c ?? "",
            ShareAddress = "", // TODO: Implement ShareAddress
            Gender = account.PersonGenderIdentity ?? ""
        };
    }
}
