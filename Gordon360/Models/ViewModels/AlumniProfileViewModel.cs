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
            ID: alu.ID.Trim(),
            WebUpdate: alu.WebUpdate ?? 0,
            Title: alu.Title ?? "",
            FirstName: alu.FirstName ?? "",
            MiddleName: alu.MiddleName ?? "",
            LastName: alu.LastName ?? "",
            Suffix: alu.Suffix ?? "",
            MaidenName: alu.MaidenName ?? "",
            NickName: alu.NickName ?? "", // Just in case some random record has a null user_name 
            HomeStreet1: alu.HomeStreet1 ?? "",
            HomeStreet2: alu.HomeStreet2 ?? "",
            HomeCity: alu.HomeCity ?? "",
            HomeState: alu.HomeState ?? "",
            HomePostalCode: alu.HomePostalCode ?? "",
            HomeCountry: alu.HomeCountry ?? "",
            HomePhone: alu.HomePhone ?? "",
            HomeEmail: alu.HomeEmail ?? "",
            JobTitle: alu.JobTitle ?? "",
            MaritalStatus: alu.MaritalStatus ?? "",
            SpouseName: alu.SpouseName ?? "",
            College: alu.College ?? "",
            ClassYear: alu.ClassYear ?? "",
            PreferredClassYear: alu.PreferredClassYear ?? "",
            Major: alu.Major1 ?? "",
            Major2: alu.Major2 ?? "",
            ShareName: alu.ShareName ?? "",
            ShareAddress: alu.ShareAddress ?? "",
            Gender: alu.Gender ?? "",
            GradDate: alu.GradDate ?? "",
            Email: alu.Email ?? "",
            grad_student: alu.grad_student ?? "",
            Barcode: alu.Barcode ?? "",
            AD_Username: alu.AD_Username ?? "", // Just in case some random record has a null email field
            show_pic: alu.show_pic,
            preferred_photo: alu.preferred_photo,
            Country: alu.Country ?? "",
            Major1Description: alu.Major1Description ?? "",
            Major2Description: alu.Major2Description ?? ""
        );
    }

    public static explicit operator AlumniProfileViewModel(Account account)
    {
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
            ID: account.Student_Id__pc ?? "",
            WebUpdate: 1, // TODO: Implement WebUpdate
            Title: account.PersonTitle ?? "",
            FirstName: account.FirstName ?? "",
            MiddleName: account.MiddleName ?? "",
            LastName: account.LastName ?? "",
            Suffix: account.Suffix__pc ?? "",
            MaidenName: account.FormerLastName__pc ?? "",
            NickName: account.Preferred_First_Name_Formula__pc ?? "", // Just in case some random record has a null user_name 
            HomeStreet1: "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            HomeStreet2: homeAddress.Street ?? "",
            HomeCity: homeAddress.City ?? "",
            HomeState: homeAddress.StateCode ?? "",
            HomePostalCode: homeAddress.PostalCode ?? "",
            HomeCountry: homeAddress.CountryCode ?? "",
            HomePhone: homeAddress.PhoneNumber,
            HomeEmail: "", // TODO: Implement home email
            JobTitle: currentEmployment?.Position ?? "",
            MaritalStatus: contact.MaritalStatus ?? "",
            SpouseName: "test test alumni", // TODO: implement spouse name
            College: "", // TODO: Implement college
            ClassYear: "", // TODO: Implement ClassYear
            PreferredClassYear: contact.gc_Preferred_Class__c ?? "",
            Major: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            Major2: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            ShareName: "", // TODO: Implement ShareName
            ShareAddress: "", // TODO: Implement ShareAddress
            Gender: account.PersonGenderIdentity ?? "",
            GradDate: "", // TODO: Implement GradDate
            Email: account.PersonEmail ?? "",
            grad_student: "", // TODO: Implement grad_student
            Barcode: "", // TODO: Implement barcode
            AD_Username: account.AD_Username__pc ?? "", // Just in case some random record has a null email field
            show_pic: 2, // show_pic
            preferred_photo: 1, // preferred_photo
            Country: "", // TODO: Implement country
            Major1Description: majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            Major2Description: majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? ""
        );
    }
}