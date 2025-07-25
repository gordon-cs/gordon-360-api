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
public class ScheduleController(IScheduleService scheduleService) : GordonControllerBase
{
    /// <summary>
    /// Gets all term-based course schedules for a user, filtered to only include officially published terms.
    /// </summary>
    /// <returns>A list of published term schedule objects</returns>
    [HttpGet]
    [Route("{username}/allcourses-by-term")]
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

        var publishedResult = result
            .Where(r => string.Equals(r.ShowOnWeb, "B", StringComparison.OrdinalIgnoreCase))
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