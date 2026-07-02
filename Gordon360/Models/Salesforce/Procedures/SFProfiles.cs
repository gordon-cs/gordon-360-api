using Gordon360.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Models.Salesforce;

public class SFProfiles
{
    private readonly SalesforceContext _context;

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
            (
                SELECT 
                    Name,
                    LearningProgramPlan.LearningProgram.Type__c,
                    LearningProgramPlan.LearningProgram.Name,
                    LearningProgramPlan.LearningProgram.gc_Jenz_Major_Minor_Code__c,
                    Status
                FROM LearnerPrograms
            ),
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

                    gc_On_Campus_Location__r.Name,
                    gc_On_Campus_Location__r.ParentLocation.Name,
                    gc_On_Campus_Location__r.gc_Jenz_Building_Code__c,
                    gc_On_Campus_Location__r.Phone,
                    gc_On_Campus_Location__r.gc_Jenz_Room_Code__c,

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
            AND Name = '{0}'
    """;

    public SFProfiles(SalesforceContext context)
    {
        _context = context;
    }

    public async Task<StudentProfileViewModel?> GetProfile(string username)
    {
        var name = (username == "360.StudentTest" || username == "")
            ? "Jamie Berry"
            : username;

        var response = await _context.Query<Account>(string.Format(SoqlTemplate, name));

        var account = response?.records?.FirstOrDefault();

        return account == null ? null : MapToViewModel(account);
    }

    private static StudentProfileViewModel MapToViewModel(Account account)
    {
        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();
        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();
        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home") 
                            ?? new ContactPointAddress();

        
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

        return new StudentProfileViewModel(
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
        );
    }
}