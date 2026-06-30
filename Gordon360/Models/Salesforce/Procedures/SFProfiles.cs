using System.Linq;

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
            Suffix__pc,
            (
                SELECT 
                    Name,
                    LearningProgramPlan.LearningProgram.Type__c,
                    LearningProgramPlan.LearningProgram.Name,
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
                    gc_On_Campus_Location__r.Name,
                    gc_On_Campus_Location__r.ParentLocation.Name,
                    gc_On_Campus_Location__r.gc_Jenz_Building_Code__c,
                    gc_On_Campus_Location__r.Phone,
                    gc_On_Campus_Location__r.gc_Jenz_Room_Code__c,
                    gc_Status__c
                FROM ContactPointAddresses
                WHERE AddressType = 'On-Campus'
            ),
            (
                SELECT 
                    Name,
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

    public async Task<StudentProfileViewModel> GetProfile(string username)
    {
        var name = username == "360.StudentTest"
            ? "Jamie Berry"
            : username;

        var response = await _context.Query<Account>(string.Format(SoqlTemplate, name));

        // first or default
        return response?.records?.FirstOrDefault() != null ? MapToViewModel(response.records.First()) : null;
    }

    private static StudentProfileViewModel MapToViewModel(Account account)
    {
        var address =
            account.ContactPointAddresses?.records?.FirstOrDefault();

        var advisor =
            account.Contacts?.records?
                .SelectMany(c => c.CCRContacts?.records ?? [])
                .FirstOrDefault(c => c.PartyRoleRelation?.Name?.Contains("Advisor") == true);

        var majors =
            account.LearnerPrograms?.records?
                .Where(x => x.LearningProgramPlan?.LearningProgram?.Type__c == "Major")
                .Take(3)
                .ToList()
            ?? [];

        var minors =
            account.LearnerPrograms?.records?
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
            account.MaidenName ?? "",
            account.NickName ?? "",
            account.OnOffCampus ?? "",
            address?.gc_On_Campus_Location__r?.gc_Jenz_Building_Code__c ?? "",
            address?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            address?.gc_On_Campus_Location__r?.Phone ?? "",
            account.OnCampusPrivatePhone ?? "",
            account.OnCampusFax ?? "",
            account.OffCampusStreet1 ?? "",
            account.OffCampusStreet2 ?? "",
            account.OffCampusCity ?? "",
            account.OffCampusState ?? "",
            account.OffCampusPostalCode ?? "",
            account.OffCampusCountry ?? "",
            account.OffCampusPhone ?? "",
            account.OffCampusFax ?? "",
            account.HomeStreet1 ?? "",
            account.HomeStreet2 ?? "",
            account.HomeCity ?? "",
            account.HomeState ?? "",
            account.HomePostalCode ?? "",
            account.HomeCountry ?? "",
            account.HomePhone ?? "",
            account.HomeFax ?? "",
            account.Cohort ?? "",
            account.Class ?? "",
            account.KeepPrivate ?? "",
            account.Barcode ?? "",
            advisor?.RelatedContact?.Name ?? "",
            account.Married ?? "",
            account.Commuter ?? "",
            majors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            majors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(0)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(1)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            minors.ElementAtOrDefault(2)?.LearningProgramPlan?.LearningProgram?.Name ?? "",
            account.PersonEmail ?? "",
            account.Gender ?? "",
            account.grad_student ?? "",
            account.GradDate ?? "",
            account.PlannedGradYear ?? "",
            account.Entrance_Date ?? "",
            account.MobilePhone ?? "",
            account.IsMobilePhonePrivate ?? "",
            account.AD_Username ?? "",
            account.show_pic ?? "",
            account.preferred_photo ?? "",
            account.Country ?? "",
            address?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            majors.ElementAtOrDefault(0)?.Name ?? "",
            majors.ElementAtOrDefault(1)?.Name ?? "",
            majors.ElementAtOrDefault(2)?.Name ?? "",
            minors.ElementAtOrDefault(0)?.Name ?? "",
            minors.ElementAtOrDefault(1)?.Name ?? "",
            minors.ElementAtOrDefault(2)?.Name ?? "",
            account.Mail_Location ?? "",
            account.ChapelRequired ?? "",
            account.ChapelAttended ?? ""
        );
    }
}