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
    string HomeFax,
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
    int? ChapelAttended)
{
    public static implicit operator StudentProfileViewModel?(Student? stu)
    {
        if (stu == null)
        {
            return null;
        }

        return new StudentProfileViewModel(
            stu.ID.Trim(),
            stu.Title ?? "",
            stu.FirstName ?? "",
            stu.MiddleName ?? "",
            stu.LastName ?? "",
            stu.Suffix ?? "",
            stu.MaidenName ?? "",
            stu.NickName ?? "", // Just in case some random record has a null user_name 
            stu.OnOffCampus ?? "",
            stu.OnCampusBuilding ?? "",
            stu.OnCampusRoom ?? "",
            stu.OnCampusPhone ?? "",
            stu.OnCampusPrivatePhone ?? "",
            stu.OnCampusFax ?? "",
            stu.OffCampusStreet1 ?? "",
            stu.OffCampusStreet2 ?? "",
            stu.OffCampusCity ?? "",
            stu.OffCampusState ?? "",
            stu.OffCampusPostalCode ?? "",
            stu.OffCampusCountry ?? "",
            stu.OffCampusPhone ?? "",
            stu.OffCampusFax ?? "",
            stu.HomeStreet1 ?? "",
            stu.HomeStreet2 ?? "",
            stu.HomeCity ?? "",
            stu.HomeState ?? "",
            stu.HomePostalCode ?? "",
            stu.HomeCountry ?? "",
            stu.HomePhone ?? "",
            stu.HomeFax ?? "",
            stu.Cohort ?? "",
            stu.Class ?? "",
            stu.KeepPrivate ?? "",
            stu.Barcode ?? "",
            stu.AdvisorIDs ?? "",
            stu.Married ?? "",
            stu.Commuter ?? "",
            stu.Major ?? "",
            stu.Major2 ?? "",
            stu.Major3 ?? "",
            stu.Minor1 ?? "",
            stu.Minor2 ?? "",
            stu.Minor3 ?? "",
            stu.Email ?? "",
            stu.Gender ?? "",
            stu.grad_student ?? "",
            stu.GradDate ?? "",
            stu.PlannedGradYear ?? "",
            stu.Entrance_Date,
            stu.MobilePhone ?? "",
            stu.IsMobilePhonePrivate == 1 ? true : false,
            stu.AD_Username ?? "", // Just in case some random record has a null email field
            stu.show_pic,
            stu.preferred_photo,
            stu.Country ?? "",
            stu.BuildingDescription ?? "",
            stu.Major1Description ?? "",
            stu.Major2Description ?? "",
            stu.Major3Description ?? "",
            stu.Minor1Description ?? "",
            stu.Minor2Description ?? "",
            stu.Minor3Description ?? "",
            stu.Mail_Location ?? "",
            stu.ChapelRequired ?? 0,
            stu.ChapelAttended ?? 0
        );
    }

    public static explicit operator StudentProfileViewModel?(Account? account)
    {
        if (account == null)
        {
            return null;
        }

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
            account.Student_Id__pc ?? "",
            account.PersonTitle ?? "",
            account.FirstName ?? "",
            account.MiddleName ?? "",
            account.LastName ?? "",
            account.Suffix__pc ?? "",
            account.FormerLastName__pc ?? "",
            account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name 
            "", // TODO: implement OnOffCampus
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Building_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            "", // TODO: implement OnCampusPrivatePhone
            "", // TODO: implement OnCampusFax
            "", // TODO: implement OffCampusStreet1
            "", // TODO: implement OffCampusStreet2
            "", // TODO: implement OffCampusCity
            "", // TODO: implement OffCampusState
            "", // TODO: implement OffCampusPostalCode
            "", // TODO: implement OffCampusCountry
            "", // TODO: implement OffCampusPhone
            "", // TODO: implement OffCampusFax
            "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            homeAddress.Street ?? "",
            homeAddress.City ?? "",
            homeAddress.StateCode ?? "",
            homeAddress.PostalCode ?? "",
            homeAddress.CountryCode ?? "",
            homeAddress.PhoneNumber,
            "", // TODO: implement fax
            "", // TODO: implement Cohort
            "", // TODO: implement Class
            "", // TODO: implement KeepPrivate
            "", // TODO: implement Barcode
            advisorsIds,
            "", // TODO: implement Married
            "", // TODO: implement Commuter
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            account.PersonEmail ?? "",
            account.PersonGenderIdentity ?? "",
            "", // TODO: implement grad_student
            "", // TODO: implement GradDate
            "", // TODO: implement PlannedGradYear
            new DateTime(1900, 1, 1), // TODO: implement EntranceDate
            contact.Phone ?? "",
            false, // TODO: implement IsMobilePhonePrivate
            account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            2, // show_pic
            1, // preferred_photo
            "", // TODO: implement Country
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            "", // TODO: implement Mail_Location
            20, // TODO: implement ChapelRequired
            5 // TODO: implement ChapelAttended
        );
    }
}





