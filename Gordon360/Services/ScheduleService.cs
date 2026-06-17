using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.Salesforce.Context;

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
        List<UserCoursesViewModel> courses = await context.UserCourses
            .Where(x => x.Username == username && x.Role == "Instructor")
            .Select(c => (UserCoursesViewModel)c)
            .ToListAsync();

        IEnumerable<YearTermTableViewModel> terms = await academicTermService.GetAllTermsAsync();

        var coursesByTerm = terms
            .GroupJoin(courses,
                       term => new { term.YearCode, term.TermCode },
                       course => new { YearCode = course.YR_CDE, TermCode = course.TRM_CDE },
                       (term, matchingCourses) => new CoursesByTermViewModel(term, matchingCourses))
            .Where(cbt => cbt.AllCourses.Any());

        return coursesByTerm.OrderByDescending(cbt => cbt.TermBeginDate);
    }

    public async Task<List<UserCoursesViewModel>> GetUserCourses(string username)
    {
        var soql = @$"SELECT
                        Name,
                        LearningCourse.SubjectAbbreviation,
                        LearningCourse.CourseNumber,
                        AcademicSession.AcademicTerm.Name,

                        (
                            SELECT
                                ParticipantAffiliation,
                                ParticipationStatus
                            FROM CourseOfferingParticipants
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
                                EndDate
                            FROM CourseOfferingSchedules
                        )

                    FROM CourseOffering
                    WHERE Id IN (
                        SELECT CourseOfferingId
                        FROM CourseOfferingParticipant
                        WHERE ParticipantContact.Name LIKE '%Sophia%'
                    )";

        var json = await salesforceContext.QueryJson(soql);

        var root = JObject.Parse(json);

        var results = new List<UserCoursesViewModel>();

        foreach (var course in root["records"]!)
        {
            var firstSchedule =
                course["CourseOfferingSchedules"]?["records"]?.FirstOrDefault();

            var participant =
                course["CourseOfferingParticipants"]?["records"]?.FirstOrDefault();

            results.Add(new UserCoursesViewModel
            {
                SessionCode =
                    course["AcademicSession"]?["AcademicTerm"]?["Name"]?.ToString(),

                YR_CDE = "2026",

                TRM_CDE =
                    course["AcademicSession"]?["AcademicTerm"]?["Name"]?.ToString(),

                CRS_CDE =
                    $"{course["LearningCourse"]?["SubjectAbbreviation"]}-{course["LearningCourse"]?["CourseNumber"]}",

                CRS_TITLE =
                    course["Name"]?.ToString() + "SF Test",

                BLDG_CDE =
                    firstSchedule?["Location"]?["ExternalReference"]?.ToString(),

                ROOM_CDE =
                    firstSchedule?["Description"]?.ToString(),

                MONDAY_CDE =
                    firstSchedule?["IsMonday"]?.Value<bool>() == true ? "Y" : "N",

                TUESDAY_CDE =
                    firstSchedule?["IsTuesday"]?.Value<bool>() == true ? "Y" : "N",

                WEDNESDAY_CDE =
                    firstSchedule?["IsWednesday"]?.Value<bool>() == true ? "Y" : "N",

                THURSDAY_CDE =
                    firstSchedule?["IsThursday"]?.Value<bool>() == true ? "Y" : "N",

                FRIDAY_CDE =
                    firstSchedule?["IsFriday"]?.Value<bool>() == true ? "Y" : "N",

                SATURDAY_CDE =
                    firstSchedule?["IsSaturday"]?.Value<bool>() == true ? "Y" : "N",

                BEGIN_DATE =
                    firstSchedule?["StartDate"]?.Value<DateTime>(),

                END_DATE =
                    firstSchedule?["EndDate"]?.Value<DateTime>(),

                BEGIN_TIME =
                    new TimeSpan(9, 0, 0), // placeholder

                END_TIME =
                    new TimeSpan(10, 15, 0), // placeholder

                SUB_TERM_CDE = "MAIN",

                Role =
                    participant?["ParticipantAffiliation"]?.ToString()
            });
        }

        return results;
    }


}