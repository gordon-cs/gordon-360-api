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
        
        string personType = (account.gc_Current_Student__c ? "stu" : "") + (account.gc_is_Current_Alumni__c ? "alu" : "") + (account.gc_Current_Faculty__pc ? "fac" : "");

        // print onCampusAddress building 
        Console.WriteLine($"On-Campus Address: {onCampusAddress}");

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
            ID: account.Student_Id__pc ?? "",
            Title: account.FirstName ?? "",
            FirstName: account.MiddleName ?? "",
            MiddleName: account.LastName ?? "",
            LastName: account.Suffix__pc ?? "",
            Suffix: account.Suffix__pc ?? "",
            MaidenName: account.FormerLastName__pc ?? "",
            NickName: account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name
            Email: account.PersonEmail ?? "",
            Gender: account.PersonGenderIdentity ?? "",
            HomeStreet1: "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            HomeStreet2: homeAddress.Street ?? "",
            HomeCity: homeAddress.City ?? "",
            HomeState: homeAddress.StateCode ?? "",
            HomePostalCode: homeAddress.PostalCode ?? "",
            HomeCountry: homeAddress.CountryCode ?? "",
            HomePhone: homeAddress.PhoneNumber,
            HomeFax: "", // TODO: implement fax
            AD_Username: account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            show_pic: 2, // show_pic
            preferred_photo: 1, // preferred_photo
            Country: "", // TODO: Implement country
            Barcode: "", // TODO: Implement barcode
            Facebook: "", // Todo: implement Facebook
            Twitter: "", // Todo: implement Twitter
            Instagram: "", // Todo: implement Instagram
            LinkedIn: "", // Todo: implement LinkedIn
            Handshake: "", // Todo: implement Handshake
            Calendar: "", // Todo: implement Calendar

            // Student only
            OnOffCampus: account.gc_Resident_Commuter__pc, // TODO: implement OnOffCampus
            OffCampusStreet1: "", // TODO: implement OffCampusStreet1
            OffCampusStreet2: "", // TODO: implement OffCampusStreet2
            OffCampusCity: "", // TODO: implement OffCampusCity
            OffCampusState: "", // TODO: implement OffCampusState
            OffCampusPostalCode: "", // TODO: implement OffCampusPostalCode
            OffCampusCountry: "", // TODO: implement OffCampusCountry
            OffCampusPhone: "", // TODO: implement OffCampusPhone
            OffCampusFax: "", // TODO: implement OffCampusFax
            Major3: majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major3Description: majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor1: minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor1Description: minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor2: minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor2Description: minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor3: minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor3Description: minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            GradDate: "", // TODO: implement GradDate
            PlannedGradYear: "", // TODO: implement PlannedGradYear
            Entrance_Date: new DateTime(1900, 1, 1), // TODO: implement EntranceDate
            MobilePhone: contact.Phone ?? "",
            IsMobilePhonePrivate: false, // TODO: implement IsMobilePhonePrivate
            ChapelRequired: 20, // TODO: implement ChapelRequired
            ChapelAttended: 5, // TODO: implement ChapelAttended
            Cohort: "", // TODO: implement Cohort
            Class: "", // TODO: implement Class
            AdvisorIDs: advisorsIds,
            Married: "", // TODO: implement Married
            Commuter: "", // TODO: implement Commuter

            // Alumni only
            WebUpdate: "1", // TODO: Implement WebUpdate
            HomeEmail: "", // TODO: Implement HomeEmail
            MaritalStatus: contact.MaritalStatus ?? "",
            College: "", // TODO: Implement College
            ClassYear: "", // TODO: Implement ClassYear
            PreferredClassYear: contact.gc_Preferred_Class__c ?? "",
            ShareName: "", // TODO: Implement ShareName
            ShareAddress: "", // TODO: Implement ShareAddress

            // Student and Alumni only
            Major: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major1Description: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major2: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major2Description: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            grad_student: "", // TODO: Implement grad_student

            // FacStaff only
            FirstHireDt: employment?.StartDate ?? new DateTime(1900, 1, 1),
            OnCampusDepartment: "", // TODO: Implement OnCampusDepartment
            Type: "", // TODO: Implement Type
            office_hours: "test test fac staff", // TODO: Implement office hours
            Dept: "", // TODO: implement department
            Mail_Description: "", // TODO: Implement mail_description

            // FacStaff and Alumni only
            JobTitle: employment?.Position ?? "",
            SpouseName: "test test profile", // TODO: implement spouse name

            // FacStaff and Student only
            BuildingDescription: onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            Mail_Location: "", // TODO: implement Mail_Location
            OnCampusBuilding: onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            OnCampusRoom: onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            OnCampusPhone: onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            OnCampusPrivatePhone: "", // TODO: implement OnCampusPrivatePhone
            OnCampusFax: "", // TODO: Implement OnCampusFax
            KeepPrivate: "", // TODO: implement KeepPrivate

            // ProfileViewModel only
            PersonType: personType
        );
    }
}