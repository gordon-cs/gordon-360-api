using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class ScheduleController(IScheduleService scheduleService) : GordonControllerBase
{

    /// <summary>
    ///  Gets all session objects for a user
    /// </summary>
    /// <returns>A IEnumerable of session objects as well as the schedules</returns>
    [HttpGet]
    [Route("{username}/allcourses")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT_SCHEDULE)]
    public async Task<ActionResult<CoursesBySessionViewModel>> GetAllCourses(string username)
    {
        IEnumerable<CoursesBySessionViewModel> result = await scheduleService.GetAllCoursesAsync(username);
        return Ok(result);

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
        return groups.Contains(AuthGroup.Advisors);
    }
}
