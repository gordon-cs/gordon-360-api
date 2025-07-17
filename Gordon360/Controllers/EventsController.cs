using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Linq;
using Gordon360.Extensions.System;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class EventsController(IEventService eventService) : GordonControllerBase
{
    [HttpGet]
    [Route("attended/{term}")]
    public ActionResult<IEnumerable<AttendedEventViewModel>> GetEventsByTerm(string term)
    {
        //get token data from context, username is the username of current logged in person
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        var result = eventService.GetEventsForStudentByTerm(authenticatedUserUsername, term);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// This makes use of our cached request to 25Live, which stores AllEvents
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public ActionResult<IEnumerable<EventViewModel>> GetAllEvents()
    {
        var result = eventService.GetAllEvents();

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);

    }

    [HttpGet]
    [Route("claw")]
    public ActionResult<IEnumerable<EventViewModel>> GetAllChapelEvents()
    {
        var result = eventService.GetCLAWEvents();

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet]
    [Route("public")]
    public ActionResult<IEnumerable<EventViewModel>> GetAllPublicEvents()
    {
        var result = eventService.GetPublicEvents();

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    [Route("finalexams/{username}")]
    public async Task<ActionResult<IEnumerable<EventViewModel>>> GetFinalExamsForUserByTermAsync(
        string username,
        [FromQuery] DateTime termStart,
        [FromQuery] DateTime termEnd,
        [FromQuery] string yearCode,
        [FromQuery] string termCode)
    {
        var groups = AuthUtils.GetGroups(User);
        var authenticatedUsername = AuthUtils.GetUsername(User);

        IEnumerable<EventViewModel> result;
        if (authenticatedUsername.EqualsIgnoreCase(username) || groups.Contains(AuthGroup.FacStaff))
        {
            result = await eventService.GetFinalExamsForUserByTermAsync(username, termStart, termEnd, yearCode, termCode);
        }
        else
        {
            result = await eventService.GetFinalExamsForInstructorByTermAsync(username, termStart, termEnd, yearCode, termCode);
        }
        return Ok(result);
    }
}

