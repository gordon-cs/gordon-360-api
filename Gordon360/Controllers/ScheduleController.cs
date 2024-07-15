using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<CoursesBySessionViewModel>> GetAllCourses(string username)
    {
        var groups = AuthUtils.GetGroups(User);
        FacultyStaffProfileViewModel? fac = profileService.GetFacultyStaffProfileByUsername(username);
        StudentProfileViewModel? student = profileService.GetStudentProfileByUsername(username);
        AlumniProfileViewModel? alumni = profileService.GetAlumniProfileByUsername(username);
        // Everyone can see faculty schedules.
        // Some users can see student and alumni schedules,
        // but check that they can see this student or alumni.
        if ((fac != null) ||
            (accountService.CanISeeStudentSchedule(groups) &&
               (student != null &&
                accountService.CanISeeThisStudent(groups, student)) ||
               (alumni != null &&
                accountService.CanISeeAlumni(groups))))
        {
            IEnumerable<CoursesBySessionViewModel> result = await scheduleService.GetAllCoursesAsync(username);
            return Ok(result);
        }
        return Forbid();
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
