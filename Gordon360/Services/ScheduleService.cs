using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.Salesforce.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
// importing to be able to use JObject for parsing salesforce query results
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
/// </summary>
public class ScheduleService(CCTContext context, SalesforceContext salesforceContext, ISessionService sessionService, IAcademicTermService academicTermService) : IScheduleService
{
    /// <summary>
    /// Fetch the session item whose id specified by the parameter
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>CoursesBySessionViewModel if found, null if not found</returns>
    public async Task<IEnumerable<CoursesBySessionViewModel>> GetAllCoursesAsync(string username)
    {
        List<UserCoursesViewModel> courses = await context.UserCourses.Where(x => x.Username == username).Select(c => (UserCoursesViewModel)c).ToListAsync();

        IEnumerable<SessionViewModel> sessions = sessionService.GetAll();
        IEnumerable<CoursesBySessionViewModel> coursesBySession = sessions
            .GroupJoin(courses,
                       s => s.SessionCode,
                       c => c.SessionCode,
                       (session, courses) => new CoursesBySessionViewModel(session, courses))
            .Where(cbs => cbs.AllCourses.Any());

        return coursesBySession.OrderByDescending(cbs => cbs.SessionCode);
    }

    /// <summary>
    /// Fetch the classes that are taught by this user
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>CoursesBySessionViewModel if found, null if not found</returns>
    public async Task<IEnumerable<CoursesBySessionViewModel>> GetAllInstructorCoursesAsync(string username)
    {
        List<UserCoursesViewModel> courses = await context.UserCourses.Where(x => x.Username == username && x.Role == "Instructor").Select(c => (UserCoursesViewModel)c).ToListAsync();

        IEnumerable<SessionViewModel> sessions = sessionService.GetAll();
        IEnumerable<CoursesBySessionViewModel> coursesBySession = sessions
            .GroupJoin(courses,
                       s => s.SessionCode,
                       c => c.SessionCode,
                       (session, courses) => new CoursesBySessionViewModel(session, courses))
            .Where(cbs => cbs.AllCourses.Any());

        return coursesBySession.OrderByDescending(cbs => cbs.SessionCode);
    }

    /// <summary>
    /// Fetch the term item whose id specified by the parameter
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>CoursesByTermViewModel if found, null if not found</returns>
    public async Task<IEnumerable<CoursesByTermViewModel>> GetAllCoursesByTermAsync(string username)
    {
        List<UserCoursesViewModel> courses = await GetUserCourses(username);

        IEnumerable<YearTermTableViewModel> terms = await academicTermService.GetAllTermsAsync();

        var coursesByTerm = terms
            .GroupJoin(courses,
                       term => new { term.YearCode, term.TermCode},
                       course => new { YearCode = course.YR_CDE, TermCode = course.TRM_CDE },
                       (term, matchingCourses) => new CoursesByTermViewModel(term, matchingCourses))
            .Where(cbt => cbt.AllCourses.Any());

        return coursesByTerm.OrderByDescending(cbt => cbt.TermBeginDate);
    }

    /// <summary>
    /// Fetch the classes that are taught by this user
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>CoursesByTermViewModel if found, null if not found</returns>
    public async Task<IEnumerable<CoursesByTermViewModel>> GetAllInstructorCoursesByTermAsync(string username)
    {
        /*List<UserCoursesViewModel> courses = await context.UserCourses
            .Where(x => x.Username == username && x.Role == "Instructor")
            .Select(c => (UserCoursesViewModel)c)
            .ToListAsync();*/
        List<UserCoursesViewModel> courses = await GetUserCourses(username, "Teacher");

        IEnumerable<YearTermTableViewModel> terms = await academicTermService.GetAllTermsAsync();

        var coursesByTerm = terms
            .GroupJoin(courses,
                       term => new { term.YearCode, term.TermCode },
                       course => new { YearCode = course.YR_CDE, TermCode = course.TRM_CDE },
                       (term, matchingCourses) => new CoursesByTermViewModel(term, matchingCourses))
            .Where(cbt => cbt.AllCourses.Any());

        return coursesByTerm.OrderByDescending(cbt => cbt.TermBeginDate);
    }

    public async Task<List<UserCoursesViewModel>> GetUserCourses(string username, string role = "")
    {
        var nameParam = username == "360.StudentTest" ? "woobensky.pierre" : username;
        var roleCondition = role == "" ? "" : $"AND ParticipantAffiliation = '{role}'";
        var soql = @$"SELECT
    Name,
    LearningCourse.SubjectAbbreviation,
    LearningCourse.CourseNumber,
    AcademicSession.AcademicTerm.Name,
    AcademicSession.gc_Jenz_Session_Code__c,
    AcademicSession.gc_Jenz_Subterm_Code__c,
    AcademicSession.gc_Jenz_Term_Code__c,
    AcademicSession.gc_Jenz_Year_Code__c,

    (
        SELECT
            ParticipantAffiliation,
            ParticipationStatus,
            ParticipantContact.Name
            
        FROM CourseOfferingParticipants
        WHERE ParticipantContact.Email LIKE '{nameParam}%'
    ),

    (
        SELECT
            Description,
            IsSunday,
            IsMonday,
            IsTuesday,
            IsWednesday,
            IsThursday,
            IsFriday,
            IsSaturday,
            Location.ExternalReference,
            StartDate,
            EndDate,
            StartTime,
            EndTime
        FROM CourseOfferingSchedules
    )

FROM CourseOffering
WHERE Id IN (
    SELECT CourseOfferingId
    FROM CourseOfferingParticipant
    WHERE ParticipantContact.Email LIKE '{nameParam}%'
        AND (NOT (ParticipationStatus='Dropped' OR ParticipationStatus='Withdrew'))
        {roleCondition}
)";

        var json = await salesforceContext.QueryJson(soql);

        var root = JObject.Parse(json);
        

        var results = new List<UserCoursesViewModel>();

        foreach (var course in root["records"]!)
        {
            var schedules =
                course["CourseOfferingSchedules"] as JObject
                ?? new JObject(
                    new JProperty("records", new JArray())
                );

            var participants =
                course["CourseOfferingParticipants"] as JObject
                ?? new JObject(
                    new JProperty("records", new JArray())
                );

            var firstSchedule =
                schedules["records"]?.FirstOrDefault();

            var participant =
                participants["records"]?.FirstOrDefault();

            var academicSession = course["AcademicSession"] as JObject;
            var academicTerm = academicSession?["AcademicTerm"] as JObject;

            var learningCourse = course["LearningCourse"] as JObject;

            var location = firstSchedule?["Location"] as JObject;

            results.Add(new UserCourses
            {
                Username = username,

                Role =
        participant?["ParticipantAffiliation"]?.ToString() ?? "",

                YR_CDE =
        academicSession?["gc_Jenz_Year_Code__c"]?.ToString() ?? "",

                TRM_CDE =
        academicSession?["gc_Jenz_Term_Code__c"]?.ToString() ?? "",

                SUBTERM_DESC = academicSession?["gc_Jenz_Subterm_Code__c"]?.ToString() ?? "",

                SUBTERM_SORT_ORDER = null,

                CRS_CDE =
        $"{learningCourse?["SubjectAbbreviation"]?.ToString() ?? ""}-{learningCourse?["CourseNumber"]?.ToString() ?? ""}",

                CRS_TITLE =
        course["Name"]?.ToString() ?? "",

                INSTRUCTOR_ID = null,

                BLDG_CDE =
        location?["ExternalReference"]?.ToString() ?? "",

                ROOM_CDE = "",

                MONDAY_CDE =
        firstSchedule?["IsMonday"]?.Value<bool?>() == true ? "M" : "",

                TUESDAY_CDE =
        firstSchedule?["IsTuesday"]?.Value<bool?>() == true ? "T" : "",

                WEDNESDAY_CDE =
        firstSchedule?["IsWednesday"]?.Value<bool?>() == true ? "W" : "",

                THURSDAY_CDE =
        firstSchedule?["IsThursday"]?.Value<bool?>() == true ? "R" : "",

                FRIDAY_CDE =
        firstSchedule?["IsFriday"]?.Value<bool?>() == true ? "F" : "",

                SATURDAY_CDE =
        firstSchedule?["IsSaturday"]?.Value<bool?>() == true ? "S" : "",

                SUNDAY_CDE =
        firstSchedule?["IsSunday"]?.Value<bool?>() == true ? "U" : "",

                BEGIN_DATE =
        firstSchedule?["StartDate"]?.Value<DateTime?>(),

                END_DATE =
        firstSchedule?["EndDate"]?.Value<DateTime?>(),

                BEGIN_TIME =
        TimeSpan.TryParse(
            firstSchedule?["StartTime"]?.ToString()?.Replace("Z", ""),
            out var beginTime)
                ? beginTime
                : null,

                END_TIME =
        TimeSpan.TryParse(
            firstSchedule?["EndTime"]?.ToString()?.Replace("Z", ""),
            out var endTime)
                ? endTime
                : null
            });
        }

        return results;
    }


}