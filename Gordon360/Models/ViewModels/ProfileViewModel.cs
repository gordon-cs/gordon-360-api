using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record ProfileViewModel(
    // All Profiles
    string ID,
    string Title,
    string FirstName,
    string MiddleName,
    string LastName,
    string Suffix,
    string MaidenName,
    string NickName,
    string Email,
    string Gender,
    string HomeStreet1,
    string HomeStreet2,
    string HomeCity,
    string HomeState,
    string HomePostalCode,
    string HomeCountry,
    string HomePhone,
    string HomeFax,
    string AD_Username,
    int? show_pic,
    int? preferred_photo,
    string Country,
    string Barcode,
    string Facebook,
    string Twitter,
    string Instagram,
    string LinkedIn,
    string Handshake,
    string Calendar,

    // Student Only
    string OnOffCampus,
    string OffCampusStreet1,
    string OffCampusStreet2,
    string OffCampusCity,
    string OffCampusState,
    string OffCampusPostalCode,
    string OffCampusCountry,
    string OffCampusPhone,
    string OffCampusFax,
    string Major3,
    string Major3Description,
    string Minor1,
    string Minor1Description,
    string Minor2,
    string Minor2Description,
    string Minor3,
    string Minor3Description,
    string GradDate,
    string PlannedGradYear,
    DateTime? Entrance_Date,
    string MobilePhone,
    bool IsMobilePhonePrivate,
    int? ChapelRequired,
    int? ChapelAttended,
    string Cohort,
    string Class,
    string AdvisorIDs,
    string Married,
    string Commuter,

    // Alumni Only
    string? WebUpdate,
    string HomeEmail,
    string MaritalStatus,
    string College,
    string ClassYear,
    string? PreferredClassYear,
    string ShareName,
    string? ShareAddress,

    // Student And Alumni Only
    string Major,
    string Major1Description,
    string Major2,
    string Major2Description,
    string grad_student,

    // FacStaff Only
    DateTime? FirstHireDt,
    string? OnCampusDepartment,
    string? Type,
    string? office_hours,
    string Dept,
    string Mail_Description,

    // FacStaff and Alumni Only
    string JobTitle,
    string SpouseName,

    // FacStaff and Student Only
    string BuildingDescription,
    string Mail_Location,
    string OnCampusBuilding,
    string OnCampusRoom,
    string OnCampusPhone,
    string OnCampusPrivatePhone,
    string OnCampusFax,
    string KeepPrivate,

    // ProfileViewModel Only
    string PersonType
    )
{
    public static explicit operator ProfileViewModel(Account account)
    {
        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();

        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home")
                            ?? new ContactPointAddress();

        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();

        // print onCampusAddress building 
        System.Console.WriteLine($"On-Campus Address: {onCampusAddress}");

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
        
        return new ProfileViewModel
        (
            account.Student_Id__pc ?? "",
            account.FirstName ?? "",
            account.MiddleName ?? "",
            account.LastName ?? "",
            account.Suffix__pc ?? "",
            account.Suffix__pc ?? "",
            account.FormerLastName__pc ?? "",
            account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name
            account.PersonEmail ?? "",
            account.PersonGenderIdentity ?? "",
            "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            homeAddress.Street ?? "",
            homeAddress.City ?? "",
            homeAddress.StateCode ?? "",
            homeAddress.PostalCode ?? "",
            homeAddress.CountryCode ?? "",
            homeAddress.PhoneNumber,
            "", // TODO: implement fax
            account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            2, // show_pic
            1, // preferred_photo
            "", // TODO: Implement country
            "", // TODO: Implement barcode
            "", // Todo: implement Facebook
            "", // Todo: implement Twitter
            "", // Todo: implement Instagram
            "", // Todo: implement LinkedIn
            "", // Todo: implement Handshake
            "", // Todo: implement Calendar

            // Student only
            "", // TODO: implement OnOffCampus
            "", // TODO: implement OffCampusStreet1
            "", // TODO: implement OffCampusStreet2
            "", // TODO: implement OffCampusCity
            "", // TODO: implement OffCampusState
            "", // TODO: implement OffCampusPostalCode
            "", // TODO: implement OffCampusCountry
            "", // TODO: implement OffCampusPhone
            "", // TODO: implement OffCampusFax
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            "", // TODO: implement GradDate
            "", // TODO: implement PlannedGradYear
            new DateTime(1900, 1, 1), // TODO: implement EntranceDate
            contact.Phone ?? "",
            false, // TODO: implement IsMobilePhonePrivate
            20, // TODO: implement ChapelRequired
            5, // TODO: implement ChapelAttended
            "", // TODO: implement Cohort
            "", // TODO: implement Class
            advisorsIds,
            "", // TODO: implement Married
            "", // TODO: implement Commuter
            
            // Alumni only
            "1", // TODO: Implement WebUpdate
            "", // TODO: Implement HomeEmail
            contact.MaritalStatus ?? "",
            "", // TODO: Implement College
            "", // TODO: Implement ClassYear
            contact.gc_Preferred_Class__c ?? "",
            "", // TODO: Implement ShareName
            "", // TODO: Implement ShareAddress
            
            // Student and Alumni only
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            "", // TODO: Implement grad_student
            
            // FacStaff only
            employment?.StartDate ?? new DateTime(1900, 1, 1),
            "", // TODO: Implement OnCampusDepartment
            "", // TODO: Implement Type
            "test test fac staff", // TODO: Implement office hours
            "", // TODO: implement department
            "", // TODO: Implement mail_description
            
            // FacStaff and Alumni only
            employment?.Position ?? "",
            "test test profile", // TODO: implement spouse name

            // FacStaff and Student only
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            "", // TODO: implement Mail_Location
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            "", // TODO: implement OnCampusPrivatePhone
            "", // TODO: Implement OnCampusFax
            "", // TODO: implement KeepPrivate

            // ProfileViewModel only
            "" // TODO: Implement PersonType           
        );
    }
}