using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class SessionsController(ISessionService sessionService) : GordonControllerBase
{

    /// <summary>Get a list of all sessions</summary>
    /// <returns>All sessions within the database</returns>
    /// <remarks>Queries the database for all sessions, current and past</remarks>
    // GET: api/Sessions
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<SessionViewModel>> Get()
    {
        var all = sessionService.GetAll();
        return Ok(all);
    }

    /// <summary>Get one specific session specified by the id in the URL string</summary>
    /// <param name="id">The identifier for one specific session</param>
    /// <returns>The information about one specific session</returns>
    /// <remarks>Queries the database regarding a specific session with the given identifier</remarks>
    // GET: api/Sessions/5
    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public ActionResult<SessionViewModel> Get(string id)
    {
        var result = sessionService.Get(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets the current active session
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("current")]
    [AllowAnonymous]
    public ActionResult<SessionViewModel> GetCurrentSession()
    {
        var currentSession = sessionService.GetCurrentSession();
        if (currentSession == null)
        {
            return NotFound();
        }

        return Ok(currentSession);
    }

    /// <summary>
    /// Gets the most recent academic session that is either Spring or Fall
    /// </summary>
    /// <returns>The current session used to fetch final exams</returns>
    [HttpGet]
    [Route("currentFinals")]
    [AllowAnonymous]
    [Obsolete]
    public ActionResult<SessionViewModel> GetCurrentSessionForFinalExams()
    {
        var currentFinalSession = sessionService.GetCurrentSessionForFinalExams();
        if (currentFinalSession == null)
        {
            return NotFound();
        }

        return Ok(currentFinalSession);
    }


    /// <summary>
    /// Gets the days left in the current session
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("daysLeft")]
    [AllowAnonymous]
    public ActionResult<double[]> GetDaysLeftInSemester()
    {
        var days = sessionService.GetDaysLeft();
        if (days == null || (days[0] == 0 && days[1] == 0))
        {
            return NotFound();
        }

        return Ok(days);
    }
}
