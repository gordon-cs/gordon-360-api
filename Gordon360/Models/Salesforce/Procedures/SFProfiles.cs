using Gordon360.Models.ViewModels;
using Gordon360.Models.CCT;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Models.Salesforce;

public class SFProfiles
{
    private readonly SalesforceContext _context;
    private const Account account = null;

    private const string employmentSoql = """
        (
            SELECT
                StartDate
            FROM PersonEmployment
            ORDER BY StartDate ASC
            LIMIT 1
        ),
    """;

    private const string educationSoql = """
        (
            SELECT 
                Name,
                LearningProgramPlan.LearningProgram.Type__c,
                LearningProgramPlan.LearningProgram.Name,
                LearningProgramPlan.LearningProgram.gc_Jenz_Major_Minor_Code__c,
                Status
            FROM LearnerPrograms
        ),
    """;

    private const string onCampusLocationFields = """
        gc_On_Campus_Location__r.Name,
        gc_On_Campus_Location__r.ParentLocation.Name,
        gc_On_Campus_Location__r.gc_Jenz_Building_Code__c,
        gc_On_Campus_Location__r.Phone,
        gc_On_Campus_Location__r.gc_Jenz_Room_Code__c,
    """;

    private const string SoqlTemplate = """
        SELECT 
            Name,
            Student_Id__pc,
            PersonEmail,
            PersonTitle,
            FirstName,
            MiddleName,
            LastName,
            FormerLastName__pc,
            Preferred_First_Name_Formula__pc,
            PersonGenderIdentity,
            AD_Username__pc,
            Suffix__pc,
            {0}
            {1}
            (
                SELECT
                    Name,
                    Description,
                    Status,
                    RoleType
                FROM Persons
            ),
            (
                SELECT 
                    Name,
                    Academic_Term__r.Name,

                    PhoneNumber,

                    Street,
                    City,
                    State,
                    PostalCode,
                    Country,
                    StateCode,
                    CountryCode,
                    {2}
                    gc_Status__c,
                    AddressType
                FROM ContactPointAddresses
                WHERE (gc_Status__c = 'Current') AND ((AddressType = 'On-Campus') OR (AddressType = 'Home'))
            ),
            (
                SELECT 
                    Name,
                    Phone,
                    (
                        SELECT
                            Name,
                            PartyRoleRelation.Name,
                            RelatedContact.Name
                        FROM CCRContacts
                    )
                FROM Contacts
            )
        FROM Account
        WHERE RecordType.Name = 'Person Account'
            AND Name = '{3}'
    """;

    public SFProfiles(SalesforceContext context)
    {
        _context = context;

    }

   


    public async Task<StudentProfileViewModel?> GetStudentProfile(string username){
        var name = (username == "360.StudentTest" || username == "")
            ? "Jamie Berry"
            : username;

        var soql = string.Format(SoqlTemplate, educationSoql, "", onCampusLocationFields, name); 
        
        var response = await _context.Query<Account>(soql);

        var account = response?.records?.FirstOrDefault();

        return account == null ? null : MapToStudentProfileViewModel(account);
    }

    private static StudentProfileViewModel MapToStudentProfileViewModel(Account account){
        Student student = MapToBaseModel<Student>(new Student(), account);

        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();

        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();


        var address =
            account.ContactPointAddresses?.records?.FirstOrDefault();

        var advisorsIds = string.Join(",",
           account.Contacts?.records?
               .SelectMany(c => c.CCRContacts?.records ?? [])
               .Where(c => c.PartyRoleRelation?.Name?.Contains("Advisor") == true)
               .Select(c => c.RelatedContact.Name)
               .Where(name => !string.IsNullOrEmpty(name))
           ?? []);

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


        student.OnOffCampus = ""; // OnOffCampus
        student.OffCampusStreet1 = ""; // OffCampusStreet1
        student.OffCampusStreet2 = ""; // OffCampusStreet2
        student.OffCampusCity = ""; // OffCampusCity
        student.OffCampusState = ""; // OffCampusState
        student.OffCampusPostalCode = ""; // OffCampusPostalCode
        student.OffCampusCountry = ""; // OffCampusCountry
        student.OffCampusPhone = ""; // OffCampusPhone
        student.OffCampusFax = ""; // OffCampusFax  
        student.Major = majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "";
        student.Major2 = majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "";
        student.Major3 = majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "";
        student.Minor1 = minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? ""; 
        student.Minor2 = minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "";
        student.Minor3 = minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "";

        student.Major1Description = majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.Major2Description = majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.Major3Description = majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.Minor1Description = minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.Minor2Description = minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.Minor3Description = minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "";
        student.AdvisorIDs = advisorsIds; // advisor ids

        student.GradDate = ""; // grad date
        student.grad_student = ""; // grad student
        student.PlannedGradYear = ""; // planned grad year
        student.Entrance_Date = new DateTime(1900, 1, 1); // Entrance year
        student.MobilePhone = contact.Phone ?? ""; // mobile phone
        student.IsMobilePhonePrivate = 0; // is mobile private
        student.ChapelRequired = 20; // ChapelRequired
        student.ChapelAttended = 5; // ChapelAttended
        student.Cohort = ""; // cohort
        student.Class = ""; // class
        student.Married = ""; // married
        student.Commuter = ""; // commuter

        student.BuildingDescription = onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "";
        student.Mail_Location = ""; // mail location
        student.OnCampusBuilding = onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Building_Code__c ?? "";
        student.OnCampusRoom = onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "";
        student.OnCampusPhone = onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "";
        student.OnCampusPrivatePhone = "";
        student.OnCampusFax = "";
        student.KeepPrivate = ""; // keep private

        return student;
        /*
        return new T(
            account.Student_Id__pc ?? "",
            account.PersonTitle ?? "",
            account.FirstName ?? "",
            account.MiddleName ?? "",
            account.LastName ?? "",
            account.Suffix__pc ?? "",
            account.FormerLastName__pc,
            account.Preferred_First_Name_Formula__pc,
            "", // OnOffCampus
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Building_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            "", // OnCampusPrivatePhone
            "", // OnCampusFax
            "", // OffCampusStreet1
            "", // OffCampusStreet2
            "", // .........City
            "", // .........State
            "", // .........PostalCode
            "", // .........Country
            "", // .........Phone
            "", // .........Fax
            "", // street1
            homeAddress.Street ?? "",
            homeAddress.City ?? "",
            homeAddress.StateCode ?? "",
            homeAddress.PostalCode ?? "",
            homeAddress.CountryCode ?? "",
            homeAddress.PhoneNumber ?? "",
            "", // fax
            "", // cohort
            "", // class
            "", // keep private
            "", // barcode
            advisorsIds,
            "", // married
            "", // commuter
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.gc_Jenz_Major_Minor_Code__c ?? "",
            account.PersonEmail ?? "",
            account.PersonGenderIdentity ?? "",
            "", // grad Student
            "", // grad date
            "", // planned grad year
            new DateTime(1900, 1, 1), // Entrance year
            contact.Phone,
            true, // is mobile private
            account.AD_Username__pc ?? "360.StudentTest",
            1, // show pic
            2, // preferred photo
            "", // country
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            "", // mail location
            20, // ChapelRequired
            5   // ChapelAttended
        );  */

    }

    private static T MapToBaseModel<T>(T profileObj, Account account) 
    {

        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();
        
        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home") 
                            ?? new ContactPointAddress();

        dynamic profile = profileObj;

        profile.ID = account.Student_Id__pc ?? "";
        profile.Title = account.PersonTitle ?? "";
        profile.FirstName = account.FirstName ?? "";
        profile.MiddleName = account.MiddleName ?? "";
        profile.LastName = account.LastName ?? "";
        profile.Suffix = account.Suffix__pc ?? "";
        profile.MaidenName = account.FormerLastName__pc ?? "";
        profile.NickName = account.Preferred_First_Name_Formula__pc ?? "";
        profile.Email = account.PersonEmail ?? "";
        profile.Gender = account.PersonGenderIdentity ?? "";
        profile.AD_Username = account.AD_Username__pc ?? "360.StudentTest";
        profile.HomeStreet1 = ""; // It seems like, for a long time, this has represented street2 (in the database, frontend and here) #TODO: we should fix that
        profile.HomeStreet2 = homeAddress.Street ?? ""; 
        profile.HomeCity = homeAddress.City ?? "";
        profile.HomeState = homeAddress.StateCode ?? "";
        profile.HomePostalCode = homeAddress.PostalCode ?? "";
        profile.HomeCountry = homeAddress.CountryCode ?? "";
        profile.HomePhone = homeAddress.PhoneNumber ?? "";   
        profile.show_pic = 1; // show pic
        profile.preferred_photo = 2; // preferred photo
        profile.Country = ""; // country

        return profile;
        
    }
}