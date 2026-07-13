using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.Salesforce;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
/// </summary>
public class ScheduleService(CCTContext context, SFUserCourses sfUserCourses, ISessionService sessionService, IAcademicTermService academicTermService) : IScheduleService
{
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

    public async Task<IEnumerable<CoursesByTermViewModel>> GetAllCoursesByTermAsync(string username)
    {
        IEnumerable<UserCoursesViewModel> courses = await sfUserCourses.GetUserCourses(username);

        IEnumerable<YearTermTableViewModel> terms = await academicTermService.GetAllTermsAsync();

        var coursesByTerm = terms
            .GroupJoin(courses,
                       term => new { term.YearCode, term.TermCode},
                       course => new { YearCode = course.YR_CDE, TermCode = course.TRM_CDE },
                       (term, matchingCourses) => new CoursesByTermViewModel(term, matchingCourses))
            .Where(cbt => cbt.AllCourses.Any());

        return coursesByTerm.OrderByDescending(cbt => cbt.TermBeginDate);
    }

    public async Task<IEnumerable<CoursesByTermViewModel>> GetAllInstructorCoursesByTermAsync(string username)
    {
        IEnumerable<UserCoursesViewModel> courses = await sfUserCourses.GetUserCourses(username, "Teacher");

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