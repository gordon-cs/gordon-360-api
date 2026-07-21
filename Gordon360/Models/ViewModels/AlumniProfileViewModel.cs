using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record AlumniProfileViewModel
    (
    string ID,
    int WebUpdate,
    string Title,
    string FirstName,
    string MiddleName,
    string LastName,
    string Suffix,
    string MaidenName,
    string NickName,
    string HomeStreet1,
    string HomeStreet2,
    string HomeCity,
    string HomeState,
    string HomePostalCode,
    string HomeCountry,
    string HomePhone,
    string HomeFax,
    string HomeEmail,
    string JobTitle,
    string MaritalStatus,
    string SpouseName,
    string College,
    string ClassYear,
    string PreferredClassYear,
    string Major,
    string Major2,
    string ShareName,
    string ShareAddress,
    string Gender,
    string GradDate,
    string Email,
    string grad_student,
    string Barcode,
    string AD_Username,
    int? show_pic,
    int? preferred_photo,
    string Country,
    string Major1Description,
    string Major2Description)
{
    public static implicit operator AlumniProfileViewModel?(Alumni? alu)
    {
        if (alu == null)
        {
            return null;
        }

        return new AlumniProfileViewModel
        (
            alu.ID.Trim(),
            alu.WebUpdate ?? 0,
            alu.Title ?? "",
            alu.FirstName ?? "",
            alu.MiddleName ?? "",
            alu.LastName ?? "",
            alu.Suffix ?? "",
            alu.MaidenName ?? "",
            alu.NickName ?? "", // Just in case some random record has a null user_name 
            alu.HomeStreet1 ?? "",
            alu.HomeStreet2 ?? "",
            alu.HomeCity ?? "",
            alu.HomeState ?? "",
            alu.HomePostalCode ?? "",
            alu.HomeCountry ?? "",
            alu.HomePhone ?? "",
            alu.HomeFax ?? "",
            alu.HomeEmail ?? "",
            alu.JobTitle ?? "",
            alu.MaritalStatus ?? "",
            alu.SpouseName ?? "",
            alu.College ?? "",
            alu.ClassYear ?? "",
            alu.PreferredClassYear ?? "",
            alu.Major1 ?? "",
            alu.Major2 ?? "",
            alu.ShareName ?? "",
            alu.ShareAddress ?? "",
            alu.Gender ?? "",
            alu.GradDate ?? "",
            alu.Email ?? "",
            alu.grad_student ?? "",
            alu.Barcode ?? "",
            alu.AD_Username ?? "", // Just in case some random record has a null email field
            alu.show_pic,
            alu.preferred_photo,
            alu.Country ?? "",
            alu.Major1Description ?? "",
            alu.Major2Description ?? ""
        );
    }

    public static explicit operator AlumniProfileViewModel?(Account? account)
    {
        if (account == null)
        {
            return null;
        }

        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Salesforce.Contact();

        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home")
                            ?? new ContactPointAddress();

        var majors = account.LearnerPrograms?.records?
                         .Where(x => x.LearningProgramPlan?.LearningProgram?.Type__c == "Major")
                         .Take(3)
                         .ToList()
                     ?? [];

        var minors = account.LearnerPrograms?.records?
                         .Where(x => x.LearningProgramPlan?.LearningProgram?.Type__c == "Minor")
                         .Take(3)
                         .ToList()
                     ?? [];

        var currentEmployment = account.PersonEmployments?.records?.FirstOrDefault() ?? new PersonEmployment();

        return new AlumniProfileViewModel
        (
            account.Student_Id__pc ?? "",
            1, // TODO: Implement WebUpdate
            account.PersonTitle ?? "",
            account.FirstName ?? "",
            account.MiddleName ?? "",
            account.LastName ?? "",
            account.Suffix__pc ?? "",
            account.FormerLastName__pc ?? "",
            account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name 
            "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            homeAddress.Street ?? "",
            homeAddress.City ?? "",
            homeAddress.StateCode ?? "",
            homeAddress.PostalCode ?? "",
            homeAddress.CountryCode ?? "",
            homeAddress.PhoneNumber,
            "", // TODO: implement fax
            "", // TODO: Implement home email
            currentEmployment?.Position ?? "",
            contact.MaritalStatus ?? "",
            "test test alumni", // TODO: implement spouse name
            "", // TODO: Implement college
            "", // TODO: Implement ClassYear
            contact.gc_Preferred_Class__c ?? "",
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            "", // TODO: Implement ShareName
            "", // TODO: Implement ShareAddress
            account.PersonGenderIdentity ?? "",
            "", // TODO: Implement GradDate
            account.PersonEmail ?? "",
            "", // TODO: Implement grad_student
            "", // TODO: Implement barcode
            account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            2, // show_pic
            1, // preferred_photo
            "", // TODO: Implement country
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? ""
        );
    }
}