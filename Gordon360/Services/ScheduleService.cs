using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
/// </summary>
public class ScheduleService(CCTContext context) : IScheduleService
{
    private readonly ISessionService _sessionService = new SessionService(context);

    /// <summary>
    /// Fetch the session item whose id specified by the parameter
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>CoursesBySessionViewModel if found, null if not found</returns>
    [Obsolete("Use GetUserCoursesPerTerm instead")]
    public async Task<IEnumerable<CoursesBySessionViewModel>> GetAllCoursesAsync(string username)
    {
        List<UserCoursesViewModel> courses = await context.UserCourses.Where(x => x.Username == username).Select(c => (UserCoursesViewModel)c).ToListAsync();

        IEnumerable<SessionViewModel> sessions = _sessionService.GetAll();
        IEnumerable<CoursesBySessionViewModel> coursesBySession = sessions
            .GroupJoin(courses,
                       s => s.SessionCode,
                       c => c.SessionCode,
                       (session, courses) => new CoursesBySessionViewModel(session, courses))
            .Where(cbs => cbs.AllCourses.Any());

        return coursesBySession.OrderByDescending(cbs => cbs.SessionCode);
    }

    /// <summary>
    /// Get schedules for the specified user that are in the specified term.
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <param name="yearCode">The year code to retrieve courses from</param>
    /// <param name="termCode">The term code to retrieve courses from</param>
    /// <param name="subtermCode">The (optional) subterm code to retrieve courses from</param>
    /// <returns>Enumerable of schedules in which the user either attended or taught courses.</returns>
    public IEnumerable<ScheduleCourseViewModel> GetUserCoursesPerTerm(string username, string yearCode, string termCode, string? subtermCode)
    {
        var courses = context.ScheduleCourse
            .Where(c => c.Username == username && c.YR_CDE == yearCode && c.TRM_CDE == termCode);

        if (subtermCode is not null)
        {
            courses = courses.Where(c => c.SUBTERM_CDE == subtermCode); 
        }

        return courses
            .OrderBy(c => c.BeginDate)
            .Select(c => new ScheduleCourseViewModel(c));
    }

    public IEnumerable<ScheduleTerm> GetTermsContainingCoursesForUser(string username)
    {
        IQueryable<string> distinctTermsWithCourse =
            context.ScheduleCourse
                .Where(x => x.Username == username)
                .Select(x => x.YR_CDE + x.TRM_CDE + x.SUBTERM_CDE)
                .Distinct();

        IQueryable<ScheduleTerm> termsContainingCourse =
            context.ScheduleTerm.Join(distinctTermsWithCourse,
                                       term => term.YearCode + term.TermCode + term.SubtermCode,
                                       session => session,
                                       (term, session) => term);

        return termsContainingCourse
            .OrderByDescending(t => t.YearCode)
            .ThenByDescending(t => t.TermCode);
    }

    public bool CanReadStudentSchedules(string username)
    {
        var groups = AuthUtils.GetGroups(username);
        return groups.Contains(AuthGroup.Advisors);
    }
}