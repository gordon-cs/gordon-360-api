using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    [AllowAnonymous]
    public class ActivitiesController : GordonControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

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
                var res = _activityService.GetActiveActivities(isActive);
                return Ok(res);

            }
            var result = _activityService.GetActivities();
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
            var result = _activityService.GetActivityByID(activityID);
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
            var result = !_activityService.ActivityRegistrationClosed(activityID);
            return Ok(result);
        }

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetActivityTypes(string type)
        {
            var res = _activityService.GetActivityLookup(type);
            if (res is not null)
            {
                return Ok(res);
            }
            return NotFound();
        }

        /// <summary>
        /// Posts Activity into CCT.RecIM.Activity
        /// </summary>
        /// <param name="newActivity">CreateActivityViewModel object with appropriate values</param>
        /// <returns>Posted Activity</returns>
        [HttpPost]
        [Route("")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_ACTIVITY)]
        public async Task<ActionResult<ActivityViewModel>> CreateActivity(ActivityUploadViewModel newActivity)
        {
            var activity = await _activityService.PostActivityAsync(newActivity);
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
        public async Task<ActionResult<ActivityViewModel>> UpdateActivity(int activityID, ActivityPatchViewModel updatedActivity)
        {
            var activity = await _activityService.UpdateActivityAsync(activityID, updatedActivity);
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
        public async Task<ActionResult> DeleteActivityCascade(int activityID)
        {
            var res = await _activityService.DeleteActivityCascade(activityID);
            return Ok(res);
        }
    }
}
