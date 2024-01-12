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
    [Obsolete("Use GetUserSchedules instead")]
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
    /// Get schedules for the specified user.
    /// </summary>
    /// <param name="username">The AD Username of the user</param>
    /// <returns>Enumerable of schedules in which the user either attended or taught courses.</returns>
    public IEnumerable<ScheduleViewModel> GetUserSchedules(string username)
    {
        IEnumerable<ScheduleCourseViewModel> courses = context.ScheduleCourses
            .Where(x => x.Username == username)
            .Select<ScheduleCourses, ScheduleCourseViewModel>(c => c)
            .AsEnumerable();

        // Todo: improve grouping so that top-level list is of full terms, and each term contains all courses, along with a list of subterms and the courses that vccur only in that subterm.
        var coursesBySession = context.ScheduleTerms
            .AsEnumerable()
            .GroupJoin(courses,
                       s => s.YearCode + s.TermCode + s.SubTermCode,
                       c => c.YearCode + c.TermCode + c.SubTermCode,
                       (session, courses) => new ScheduleViewModel(session, courses)
            );

        return coursesBySession.Where(cbs => cbs.Courses.Any()).OrderByDescending(cbs => cbs.Session.Start);
    }

    public bool CanReadStudentSchedules(string username)
    {
        var groups = AuthUtils.GetGroups(username);
        return groups.Contains(AuthGroup.Advisors);
    }

}