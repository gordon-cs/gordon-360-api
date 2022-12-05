﻿using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        ///<param name="active"> Optional active parameter </param>
        ///<param name="time"> Optional time parameter </param>
        /// <returns>
        /// All Existing Activities 
        /// </returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<ActivityViewModel>> GetActivities([FromQuery] DateTime? time, bool active)
        {   
            if (time is null && active)
            {
                var activeResults = _activityService.GetActivities();
                return Ok(activeResults);
                
            }
            var result = _activityService.GetActivitiesByTime(time);
            return Ok(result);
        }

        ///<summary>Gets a Activity object by ID number</summary>
        /// <param name="activityID">League ID Number</param>
        /// <returns>
        /// Activity object
        /// </returns>
        [HttpGet]
        [Route("{activityID}")]
        public ActionResult<ActivityViewModel> GetActivityByID(int activityID)
        {
            var result = _activityService.GetActivityByID(activityID);
            return Ok(result);
        }

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetActivityTypes(string type)
        {
            if ( type == "status")
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Posts Activity into CCT.RecIM.Activity
        /// </summary>
        /// <param name="newActivity">CreateActivityViewModel object with appropriate values</param>
        /// <returns>Posted Activity</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ActivityCreatedViewModel>> CreateActivity(ActivityUploadViewModel newActivity)
        {
            var activity = await _activityService.PostActivity(newActivity);
            return CreatedAtAction("CreateActivity",activity);
        }
        /// <summary>
        /// Updates Activity based on input
        /// </summary>
        /// <param name="activityID"> Activity ID</param>
        /// <param name="updatedActivity"> Updated Activity Object </param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{activityID}")]
        public async Task<ActionResult<ActivityCreatedViewModel>> UpdateActivity(int activityID, ActivityPatchViewModel updatedActivity)
        {
            var activity = await _activityService.UpdateActivity(activityID, updatedActivity);
            return CreatedAtAction("UpdateActivity", activity);
        }
    }
}
