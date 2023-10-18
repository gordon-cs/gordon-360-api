using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.SCHEDULE)]
    public ActionResult<IEnumerable<ScheduleViewModel>> GetSchedules(string? username)
    {
        username ??= AuthUtils.GetUsername(User);
        return Ok(_scheduleService.GetUserSchedules(username));
    }

    /// <summary>
    ///  Gets all session objects for a user
    /// </summary>
    /// <returns>A IEnumerable of session objects as well as the schedules</returns>
    [HttpGet]
    [Route("{username}/allcourses")]
    [Obsolete("Use the basic get route instead")]
    public async Task<ActionResult<CoursesBySessionViewModel>> GetAllCourses(string username)
    {
        IEnumerable<CoursesBySessionViewModel> result = await _scheduleService.GetAllCoursesAsync(username);
        return Ok(result);

    }

    /// <summary>
    /// Get whether the currently logged-in user can read student schedules
    /// </summary>
    /// <returns>Whether they can read student schedules</returns>
    [HttpGet]
    [Route("canreadstudent")]
    public ActionResult<bool> GetCanReadStudentSchedules()
    => Ok(_scheduleService.CanReadStudentSchedules(AuthUtils.GetUsername(User)));
}
