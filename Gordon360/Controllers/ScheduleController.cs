﻿using Gordon360.Authorization;
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
public class ScheduleController(IScheduleService scheduleService) : GordonControllerBase
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
        var authenticatedUsername = AuthUtils.GetUsername(User);

        IEnumerable<CoursesBySessionViewModel> result;
        if (authenticatedUsername.EqualsIgnoreCase(username) || groups.Contains(AuthGroup.FacStaff))
        {
            result = await scheduleService.GetAllCoursesAsync(username);
        }
        else
        {
            result = await scheduleService.GetAllInstructorCoursesAsync(username);
        }

        return Ok(result);
    }

    /// <summary>
    ///  Gets all term objects for a user — only includes published schedules
    /// </summary>
    /// <returns>A IEnumerable of term objects with their schedules</returns>
    [HttpGet]
    [Route("{username}/allcourses-by-term")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT_SCHEDULE)]
    public async Task<ActionResult<IEnumerable<CoursesByTermViewModel>>> GetAllCoursesByTerm(string username)
    {
        var groups = AuthUtils.GetGroups(User);
        var authenticatedUsername = AuthUtils.GetUsername(User);

        IEnumerable<CoursesByTermViewModel> result;
        if (authenticatedUsername.EqualsIgnoreCase(username) || groups.Contains(AuthGroup.FacStaff))
        {
            result = await scheduleService.GetAllCoursesByTermAsync(username);
        }
        else
        {
            result = await scheduleService.GetAllInstructorCoursesByTermAsync(username);
        }

        // Filter only published terms
        var publishedResult = result
            .Where(r => r.ShowOnWeb?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
        return Ok(publishedResult);
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
