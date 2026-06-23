using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class SFUserCourses
{
    private readonly SalesforceContext _context;

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
                WHERE ParticipantContact.Email LIKE '{0}%'
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
            WHERE ParticipantContact.Email LIKE '{0}%'
                AND NOT (ParticipationStatus='Dropped' OR ParticipationStatus='Withdrew')
                {1}
        )
    """;

    public SFUserCourses(SalesforceContext context) => _context = context;

    public async Task<List<UserCourseViewModel>> GetUserCourses(string username, string role = "")
    {
        var name = username == "360.StudentTest" ? "woobensky.pierre" : username;
        var roleFilter = string.IsNullOrWhiteSpace(role) ? "" : $"AND ParticipantAffiliation = '{role}'";

        var json = await _context.QueryJson(string.Format(SoqlTemplate, name, roleFilter));
        var response = JsonConvert.DeserializeObject<SFQueryResult<CourseOffering>>(json);

        return response?.records?
            .Select(c => MapToViewModel(c, username))
            .ToList() ?? new List<UserCourseViewModel>();
    }

    private static UserCourseViewModel MapToViewModel(CourseOffering c, string username)
    {
        var schedule = c.CourseOfferingSchedules.records.FirstOrDefault();
        var participant = c.CourseOfferingParticipants.records.FirstOrDefault();

        return new UserCourseViewModel
        {
            Username = username,
            Role = participant?.ParticipantAffiliation ?? "",

            YR_CDE = c.AcademicSession.gc_Jenz_Year_Code__c,
            TRM_CDE = c.AcademicSession.gc_Jenz_Term_Code__c,
            SUBTERM_DESC = c.AcademicSession.gc_Jenz_Subterm_Code__c,

            CRS_CDE = $"{c.LearningCourse.SubjectAbbreviation}-{c.LearningCourse.CourseNumber}",
            CRS_TITLE = c.Name,

            BLDG_CDE = schedule?.Location.ExternalReference ?? "",

            MONDAY_CDE = DayCode(schedule?.IsMonday, "M"),
            TUESDAY_CDE = DayCode(schedule?.IsTuesday, "T"),
            WEDNESDAY_CDE = DayCode(schedule?.IsWednesday, "W"),
            THURSDAY_CDE = DayCode(schedule?.IsThursday, "R"),
            FRIDAY_CDE = DayCode(schedule?.IsFriday, "F"),
            SATURDAY_CDE = DayCode(schedule?.IsSaturday, "S"),
            SUNDAY_CDE = DayCode(schedule?.IsSunday, "U"),

            BEGIN_DATE = schedule?.StartDate,
            END_DATE = schedule?.EndDate,

            BEGIN_TIME = ParseTime(schedule?.StartTime),
            END_TIME = ParseTime(schedule?.EndTime)
        };
    }

    private static string DayCode(bool? flag, string code) => flag == true ? code : "";

    private static TimeSpan? ParseTime(string? time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return null;
        }
        else
        {
            var cleanedTime = time.Replace("Z", "");
            var isValid = TimeSpan.TryParse(cleanedTime, out var t);

            return isValid ? t : null;
        }
    }
}