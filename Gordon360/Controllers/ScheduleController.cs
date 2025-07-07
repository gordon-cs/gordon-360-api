using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Extensions.System;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class ScheduleController(IProfileService profileService,
                                IScheduleService scheduleService,
                                IAccountService accountService) : GordonControllerBase
{
    /// <summary>
    ///  Gets all session objects for a user
    /// </summary>
    /// <returns>A IEnumerable of session objects as well as the schedules</returns>
    [HttpGet]
    [Route("{username}/allcourses")]
    [Obsolete("This method is deprecated. Use '/{username}/allcourses-by-term' which is grouped by term.")]
    public async Task<ActionResult<CoursesBySessionViewModel>> GetAllCourses(string username)
    {
        var groups = AuthUtils.GetGroups(User);
        FacultyStaffProfileViewModel? fac = profileService.GetFacultyStaffProfileByUsername(username);
        StudentProfileViewModel? student = profileService.GetStudentProfileByUsername(username);
        AlumniProfileViewModel? alumni = profileService.GetAlumniProfileByUsername(username);

        // Some users can see schedules of courses taken, as well as taught,
        // so check to see if this user can see all courses for this person.
        if ((accountService.CanISeeStudentSchedule(groups) &&
               student != null &&
               accountService.CanISeeThisStudent(groups, student)) ||
            (alumni != null && accountService.CanISeeAlumni(groups)))
        {
            IEnumerable<CoursesByTermViewModel> result = await scheduleService.GetAllCoursesAsync(username);
            return Ok(result);
        }
        else
        {
            // Everyone can see schedules of courses taught.
            IEnumerable<CoursesByTermViewModel> result = await scheduleService.GetAllInstructorCoursesAsync(username);
            return Ok(result);
        }
    }

    /// <summary>
    ///  Gets all visible course objects for a user, for all visible terms
    /// </summary>
    /// <returns>A IEnumerable of term objects as well as the schedules</returns>
    [HttpGet]
    [Route("{username}/allcourses-by-term")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT_SCHEDULE)]
    public async Task<ActionResult<IEnumerable<CoursesByTermViewModel>>> GetAllCoursesByTerm(string username)
    {
        var groups = AuthUtils.GetGroups(User);
        FacultyStaffProfileViewModel? fac = profileService.GetFacultyStaffProfileByUsername(username);
        StudentProfileViewModel? student = profileService.GetStudentProfileByUsername(username);
        AlumniProfileViewModel? alumni = profileService.GetAlumniProfileByUsername(username);

        // Some users can see schedules of courses taken, as well as taught,
        // so check to see if this user can see all courses for this person.
        if ((accountService.CanISeeStudentSchedule(groups) &&
               student != null && 
               accountService.CanISeeThisStudent(groups, student)) ||
            (alumni != null && accountService.CanISeeAlumni(groups)))
        {
            IEnumerable<CoursesByTermViewModel> result = await scheduleService.GetAllCoursesByTermAsync(username);
            return Ok(result);
        } else
        {
            // Everyone can see schedules of courses taught.
            IEnumerable<CoursesByTermViewModel> result = await scheduleService.GetAllInstructorCoursesByTermAsync(username);
            return Ok(result);
        }
    }

    /// <summary>
    /// Get whether the currently logged-in user can read student schedules
    /// </summary>
    /// <returns>Whether they can read student schedules</returns>
    [HttpGet]
    [Route("canreadstudent")]
    public async Task<ActionResult<bool>> GetCanReadStudentSchedules()
    {
        var groups = AuthUtils.GetGroups(User);
        return accountService.CanISeeStudentSchedule(groups);
    }
}
