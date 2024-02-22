using Gordon360.Authorization;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

[Route("api/recim/[controller]")]
public class ActivitiesController(IActivityService activityService) : GordonControllerBase
{

    ///<summary>Gets a list of all Activities by parameter </summary>
    ///<param name="active"> Optional active parameter denoting whether or not an activity has been completed </param>
    /// <returns>
    /// All Existing Activities 
    /// </returns>
    [HttpGet]
    [Route("")]
    public ActionResult<IEnumerable<ActivityExtendedViewModel>> GetActivities([FromQuery] bool? active)
    {   
        if ( active is bool isActive)
        {
            bool completed = !isActive;
            var res = activityService.GetActivitiesByCompletion(completed);
            return Ok(res);

        }
        var result = activityService.GetActivities();
        return Ok(result);
    }

    ///<summary>Gets a Activity object by ID number</summary>
    /// <param name="activityID">Activity ID Number</param>
    /// <returns>
    /// Activity object
    /// </returns>
    [HttpGet]
    [Route("{activityID}")]
    public ActionResult<ActivityExtendedViewModel> GetActivityByID(int activityID)
    {
        var result = activityService.GetActivityByID(activityID);
        return Ok(result);
    }

    /// <summary>
    /// Niche function to check if the activity is still open for registration
    /// </summary>
    /// <param name="activityID"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{activityID}/registerable")]
    public ActionResult<bool> GetActivityRegistrationStatus(int activityID)
    {
        var result = !activityService.ActivityRegistrationClosed(activityID);
        return Ok(result);
    }

    [HttpGet]
    [Route("lookup")]
    public ActionResult<IEnumerable<LookupViewModel>> GetActivityTypes(string type)
    {
        var res = activityService.GetActivityLookup(type);
        if (res is not null)
        {
            return Ok(res);
        }
        return NotFound();
    }

    /// <summary>
    /// Posts Activity into Gordon360.RecIM.Activity
    /// </summary>
    /// <param name="newActivity">CreateActivityViewModel object with appropriate values</param>
    /// <returns>Posted Activity</returns>
    [HttpPost]
    [Route("")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_ACTIVITY)]
    public async Task<ActionResult<ActivityViewModel>> CreateActivityAsync(ActivityUploadViewModel newActivity)
    {
        var activity = await activityService.PostActivityAsync(newActivity);
        return CreatedAtAction(nameof(GetActivityByID), new { activityID = activity.ID }, activity);
    }

    /// <summary>
    /// Updates Activity based on input
    /// </summary>
    /// <param name="activityID"> Activity ID</param>
    /// <param name="updatedActivity"> Updated Activity Object </param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{activityID}")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_ACTIVITY)]
    public async Task<ActionResult<ActivityViewModel>> UpdateActivityAsync(int activityID, ActivityPatchViewModel updatedActivity)
    {
        var activity = await activityService.UpdateActivityAsync(activityID, updatedActivity);
        return CreatedAtAction(nameof(GetActivityByID), new { activityID = activity.ID }, activity);
    }

    /// <summary>
    /// Cascade deletes all DBobjects related to given ActivityID
    /// </summary>
    /// <param name="activityID"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{activityID}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_ACTIVITY)]
    public async Task<ActionResult> DeleteActivityCascadeAsync(int activityID)
    {
        var res = await activityService.DeleteActivityCascade(activityID);
        return Ok(res);
    }
}
