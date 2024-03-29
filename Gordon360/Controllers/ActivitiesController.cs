﻿using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class ActivitiesController(CCTContext context, IActivityService activityService) : GordonControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<ActivityInfoViewModel>> Get()
    {
        var all = activityService.GetAll();
        return Ok(all);
    }

    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public ActionResult<ActivityInfoViewModel> Get(string id)
    {
        var result = activityService.Get(id);

        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>Gets the activities taking place during a given session</summary>
    /// <param name="id">The session identifier</param>
    /// <returns>A list of all activities that are active during the given session determined by the id parameter</returns>
    /// <remarks>Queries the database to find which activities are active during the session desired</remarks>
    // GET: api/sessions/id/activities
    [HttpGet]
    [Route("session/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ActivityInfoViewModel>>> GetActivitiesForSessionAsync(string id)
    {
        var result = await activityService.GetActivitiesForSessionAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);

    }

    /// <summary>Gets the different types of activities taking place during a given session</summary>
    /// <param name="id">The session identifier</param>
    /// <returns>A list of all the different types of activities that are active during the given session determined by the id parameter</returns>
    /// <remarks>Queries the database to find the distinct activities type of activities that are active during the session desired</remarks>
    [HttpGet]
    [Route("session/{id}/types")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetActivityTypesForSessionAsync(string id)
    {
        var result = await activityService.GetActivityTypesForSessionAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);

    }

    /// <summary>
    /// Get the status (open or closed) of an activity for a given session
    /// </summary>
    /// <param name="sessionCode">The session code that we want to check the status for</param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{sessionCode}/{id}/status")]
    [AllowAnonymous]
    public ActionResult<bool> GetActivityStatus(string sessionCode, string id)
    {
        var result = activityService.IsOpen(id, sessionCode) ? "OPEN" : "CLOSED";

        return Ok(result);
    }

    /// <summary>
    /// Get all activities that have not yet been closed out for the current session
    /// </summary>
    /// <returns>Enumerable of ActivityInfo for each open activity in the current session</returns>
    [HttpGet]
    [Route("open")]
    public ActionResult<IEnumerable<ActivityInfoViewModel>> GetOpenActivities()
    {
        var sessionCode = Helpers.GetCurrentSession(context);

        var activities = activityService.GetActivitiesByStatus(sessionCode, getOpen: true);

        return Ok(activities);
    }

    /// <summary>
    /// Get all the activities that are already closed out for the current session
    /// </summary>
    /// <returns>Enumerable of ActivityInfo for each closed activity in the current session</returns>
    [HttpGet]
    [Route("closed")]
    public ActionResult<IEnumerable<ActivityInfoViewModel>> GetClosedActivities()
    {
        var sessionCode = Helpers.GetCurrentSession(context);

        var activities = activityService.GetActivitiesByStatus(sessionCode, getOpen: false);

        return Ok(activities);
    }

    /// <summary>
    /// </summary>
    /// <param name="involvement_code">The code of the activity to update</param>
    /// <param name="involvement">The updated involvement details</param>
    /// <returns></returns>
    [HttpPut]
    [Route("{involvement_code}")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
    public ActionResult<ActivityInfoViewModel> Put(string involvement_code, InvolvementUpdateViewModel involvement)
    {
        var result = activityService.Update(involvement_code, involvement);

        if (result == null)
        {
            return NotFound();
        }

        return Ok((ActivityInfoViewModel)result);
    }

    [HttpPut]
    [Route("{id}/session/{sess_cde}/close")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
    public ActionResult CloseSession(string id, string sess_cde)
    {
        activityService.CloseOutActivityForSession(id, sess_cde);

        return Ok();
    }

    [HttpPut]
    [Route("{id}/session/{sess_cde}/open")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
    public ActionResult OpenSession(string id, string sess_cde)
    {
        activityService.OpenActivityForSession(id, sess_cde);

        return Ok();
    }

    /// <summary>
    /// Set an image for the activity
    /// </summary>
    /// <param name="involvement_code">The activity code</param>
    /// <param name="image">The image file</param>
    /// <returns></returns>
    [HttpPost]
    [Route("{involvement_code}/image")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
    public async Task<ActionResult<ActivityInfoViewModel>> PostImageAsync(string involvement_code, [FromForm] IFormFile image)
    {
        var involvement = await context.ACT_INFO.FindAsync(involvement_code);
        if (involvement is null)
        {
            return NotFound("Involvement not found");
        }

        ActivityInfoViewModel updatedInvolvement = await activityService.UpdateActivityImageAsync(involvement, image);
        
        return Ok(updatedInvolvement);
    }

    /// <summary>
    /// Reset the activity Image
    /// </summary>
    /// <param name="involvement_code">The activity code</param>
    /// <returns></returns>
    [HttpPost]
    [Route("{involvement_code}/image/reset")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
    public ActionResult ResetImage(string involvement_code)
    {
        activityService.ResetActivityImage(involvement_code);

        return Ok();
    }

    /// <summary>Update an existing activity to be private or not</summary>
    /// <param name="involvement_code">The code of the involvement</param>
    /// <param name = "p">the boolean value</param>
    /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
    [HttpPut]
    [Route("{involvement_code}/privacy/{p}")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
    public ActionResult TogglePrivacy(string involvement_code, bool p)
    {
        activityService.TogglePrivacy(involvement_code, p);
        return Ok();
    }

}
