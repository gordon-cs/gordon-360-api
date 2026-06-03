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
public class ScheduleService(CCTContext context, ISessionService sessionService, IAcademicTermService academicTermService) : IScheduleService
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
        List<UserCoursesViewModel> courses = await context.UserCourses
            .Where(x => x.Username == username)
            .Select(c => (UserCoursesViewModel)c)
            .ToListAsync();

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
}