using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class SFUserCourses(ISalesforceContext context)
{
    private readonly ISalesforceContext _context = context;

    private const string SoqlTemplate = """
        SELECT
            Name,
            LearningCourse.SubjectAbbreviation,
            LearningCourse.CourseNumber,
            AcademicSession.AcademicTerm.Name,
            AcademicSession.gc_Jenz_Session_Code__c,
            AcademicSession.gc_Jenz_Subterm_Code__c,
            AcademicSession.gc_Jenz_Term_Code__c,
            AcademicSession.gc_Jenz_Year_Code__c,
            (
                SELECT ParticipantAffiliation, ParticipationStatus, ParticipantContact.Name
                FROM CourseOfferingParticipants
                WHERE ParticipantContact.gc_University_Email__c LIKE '{0}%'
            ),
            (
                SELECT Description, IsSunday, IsMonday, IsTuesday, IsWednesday, IsThursday, IsFriday, IsSaturday,
                       Location.ExternalReference, StartDate, EndDate, StartTime, EndTime
                FROM CourseOfferingSchedules
            )
        FROM CourseOffering
        WHERE Id IN (
            SELECT CourseOfferingId
            FROM CourseOfferingParticipant
            WHERE ParticipantContact.gc_University_Email__c LIKE '{0}%'
                AND (NOT (ParticipationStatus='Dropped' OR ParticipationStatus='Withdrew'))
                {1}
        )
    """;

    /// <summary>
    /// Fetch IEnumerable of courses taken by the given user
    /// </summary>
    /// <param name="username">Active Directory username</param>
    /// <param name="role">Role of user requesting</param>
    /// <returns>IEnumerable of courses taken by user, or empty if unauthorized</returns>
    public async Task<IEnumerable<UserCoursesViewModel>> GetUserCourses(string username, string role = "")
    {
        var roleFilter = string.IsNullOrWhiteSpace(role) ? "" : $"AND ParticipantAffiliation = '{role}'";

        var response = await _context.SoqlQuery<CourseOffering>(string.Format(SoqlTemplate, username, roleFilter));
        
        List<CourseOffering> records = response?.records ?? [];

        return records
            .Select(UserCoursesViewModel.FromCourseOffering)
            .ToList() ?? [];
    }

}