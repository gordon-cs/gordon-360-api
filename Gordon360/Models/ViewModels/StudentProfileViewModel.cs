using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record StudentProfileViewModel
    (
    string ID,
    string Title,
    string FirstName,
    string MiddleName,
    string LastName,
    string Suffix,
    string MaidenName,
    string NickName,
    string OnOffCampus,
    string OnCampusBuilding,
    string OnCampusRoom,
    string OnCampusPhone,
    string OnCampusPrivatePhone,
    string OnCampusFax,
    string OffCampusStreet1,
    string OffCampusStreet2,
    string OffCampusCity,
    string OffCampusState,
    string OffCampusPostalCode,
    string OffCampusCountry,
    string OffCampusPhone,
    string OffCampusFax,
    string HomeStreet1,
    string HomeStreet2,
    string HomeCity,
    string HomeState,
    string HomePostalCode,
    string HomeCountry,
    string HomePhone,
    string Cohort,
    string Class,
    string KeepPrivate,
    string Barcode,
    string AdvisorIDs,
    string Married,
    string Commuter,
    string Major,
    string Major2,
    string Major3,
    string Minor1,
    string Minor2,
    string Minor3,
    string Email,
    string Gender,
    string grad_student,
    string GradDate,
    string PlannedGradYear,
    DateTime? Entrance_Date,
    string MobilePhone,
    bool IsMobilePhonePrivate,
    string AD_Username,
    int? show_pic,
    int? preferred_photo,
    string Country,
    string BuildingDescription,
    string Major1Description,
    string Major2Description,
    string Major3Description,
    string Minor1Description,
    string Minor2Description,
    string Minor3Description,
    string Mail_Location,
    int? ChapelRequired,
    int? ChapelAttended
    )
{
    public static implicit operator StudentProfileViewModel?(Student? stu)
    {
        if (stu == null)
        {
            return null;
        }

        return new StudentProfileViewModel(
            ID: stu.ID.Trim(),
            Title: stu.Title ?? "",
            FirstName: stu.FirstName ?? "",
            MiddleName: stu.MiddleName ?? "",
            LastName: stu.LastName ?? "",
            Suffix: stu.Suffix ?? "",
            MaidenName: stu.MaidenName ?? "",
            NickName: stu.NickName ?? "", // Just in case some random record has a null user_name 
            OnOffCampus: stu.OnOffCampus ?? "",
            OnCampusBuilding: stu.OnCampusBuilding ?? "",
            OnCampusRoom: stu.OnCampusRoom ?? "",
            OnCampusPhone: stu.OnCampusPhone ?? "",
            OnCampusPrivatePhone: stu.OnCampusPrivatePhone ?? "",
            OnCampusFax: stu.OnCampusFax ?? "",
            OffCampusStreet1: stu.OffCampusStreet1 ?? "",
            OffCampusStreet2: stu.OffCampusStreet2 ?? "",
            OffCampusCity: stu.OffCampusCity ?? "",
            OffCampusState: stu.OffCampusState ?? "",
            OffCampusPostalCode: stu.OffCampusPostalCode ?? "",
            OffCampusCountry: stu.OffCampusCountry ?? "",
            OffCampusPhone: stu.OffCampusPhone ?? "",
            OffCampusFax: stu.OffCampusFax ?? "",
            HomeStreet1: stu.HomeStreet1 ?? "",
            HomeStreet2: stu.HomeStreet2 ?? "",
            HomeCity: stu.HomeCity ?? "",
            HomeState: stu.HomeState ?? "",
            HomePostalCode: stu.HomePostalCode ?? "",
            HomeCountry: stu.HomeCountry ?? "",
            HomePhone: stu.HomePhone ?? "",
            Cohort: stu.Cohort ?? "",
            Class: stu.Class ?? "",
            KeepPrivate: stu.KeepPrivate ?? "",
            Barcode: stu.Barcode ?? "",
            AdvisorIDs: stu.AdvisorIDs ?? "",
            Married: stu.Married ?? "",
            Commuter: stu.Commuter ?? "",
            Major: stu.Major ?? "",
            Major2: stu.Major2 ?? "",
            Major3: stu.Major3 ?? "",
            Minor1: stu.Minor1 ?? "",
            Minor2: stu.Minor2 ?? "",
            Minor3: stu.Minor3 ?? "",
            Email: stu.Email ?? "",
            Gender: stu.Gender ?? "",
            grad_student: stu.grad_student ?? "",
            GradDate: stu.GradDate ?? "",
            PlannedGradYear: stu.PlannedGradYear ?? "",
            Entrance_Date: stu.Entrance_Date,
            MobilePhone: stu.MobilePhone ?? "",
            IsMobilePhonePrivate: stu.IsMobilePhonePrivate == 1 ? true : false,
            AD_Username: stu.AD_Username ?? "", // Just in case some random record has a null email field
            show_pic: stu.show_pic,
            preferred_photo: stu.preferred_photo,
            Country: stu.Country ?? "",
            BuildingDescription: stu.BuildingDescription ?? "",
            Major1Description: stu.Major1Description ?? "",
            Major2Description: stu.Major2Description ?? "",
            Major3Description: stu.Major3Description ?? "",
            Minor1Description: stu.Minor1Description ?? "",
            Minor2Description: stu.Minor2Description ?? "",
            Minor3Description: stu.Minor3Description ?? "",
            Mail_Location: stu.Mail_Location ?? "",
            ChapelRequired: stu.ChapelRequired ?? 0,
            ChapelAttended: stu.ChapelAttended ?? 0
        );
    }

    public static explicit operator StudentProfileViewModel(Account account)
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

        return new StudentProfileViewModel(
            ID: account.Student_Id__pc ?? "",
            Title: account.PersonTitle ?? "",
            FirstName: account.FirstName ?? "",
            MiddleName: account.MiddleName ?? "",
            LastName: account.LastName ?? "",
            Suffix: account.Suffix__pc ?? "",
            MaidenName: account.FormerLastName__pc ?? "",
            NickName: account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name 
            OnOffCampus: "", // TODO: implement OnOffCampus
            OnCampusBuilding: onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Building_Code__c ?? "",
            OnCampusRoom: onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            OnCampusPhone: onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            OnCampusPrivatePhone: "", // TODO: implement OnCampusPrivatePhone
            OnCampusFax: "", // TODO: implement OnCampusFax
            OffCampusStreet1: "", // TODO: implement OffCampusStreet1
            OffCampusStreet2: "", // TODO: implement OffCampusStreet2
            OffCampusCity: "", // TODO: implement OffCampusCity
            OffCampusState: "", // TODO: implement OffCampusState
            OffCampusPostalCode: "", // TODO: implement OffCampusPostalCode
            OffCampusCountry: "", // TODO: implement OffCampusCountry
            OffCampusPhone: "", // TODO: implement OffCampusPhone
            OffCampusFax: "", // TODO: implement OffCampusFax
            HomeStreet1: "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            HomeStreet2: homeAddress.Street ?? "",
            HomeCity: homeAddress.City ?? "",
            HomeState: homeAddress.StateCode ?? "",
            HomePostalCode: homeAddress.PostalCode ?? "",
            HomeCountry: homeAddress.CountryCode ?? "",
            HomePhone: homeAddress.PhoneNumber,
            Cohort: "", // TODO: implement Cohort
            Class: "", // TODO: implement Class
            KeepPrivate: "", // TODO: implement KeepPrivate
            Barcode: "", // TODO: implement Barcode
            AdvisorIDs: advisorsIds,
            Married: "", // TODO: implement Married
            Commuter: "", // TODO: implement Commuter
            Major: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major2: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major3: majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor1: minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor2: minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Minor3: minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Email: account.PersonEmail ?? "",
            Gender: account.PersonGenderIdentity ?? "",
            grad_student: "", // TODO: implement grad_student
            GradDate: "", // TODO: implement GradDate
            PlannedGradYear: "", // TODO: implement PlannedGradYear
            Entrance_Date: new DateTime(1900, 1, 1), // TODO: implement EntranceDate
            MobilePhone: contact.Phone ?? "",
            IsMobilePhonePrivate: false, // TODO: implement IsMobilePhonePrivate
            AD_Username: account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            show_pic: 2, // show_pic
            preferred_photo: 1, // preferred_photo
            Country: "", // TODO: implement Country
            BuildingDescription: onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            Major1Description: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major2Description: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major3Description: majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor1Description: minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor2Description: minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Minor3Description: minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Mail_Location: "", // TODO: implement Mail_Location
            ChapelRequired: 20, // TODO: implement ChapelRequired
            ChapelAttended: 5 // TODO: implement ChapelAttended
        );
    }
}





