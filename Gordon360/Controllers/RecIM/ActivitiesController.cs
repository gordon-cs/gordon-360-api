using Gordon360.Models.CCT;
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


        /// <summary>
        /// Posts Activity into CCT.RecIM.Activity
        /// </summary>
        /// <param name="a">CreateActivityViewModel object with appropriate values</param>
        /// <returns>Posted Activity ID</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateActivity(CreateActivityViewModel a)
        {
            
            var activityID = await _activityService.PostActivity(a);
            return Ok(activityID);
        }
        /// <summary>
        /// Updates Activity based on input
        /// </summary>
        /// <param name="a"> Updated Activity Object </param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateActivity(UpdateActivityViewModel a)
        {
            await _activityService.UpdateActivity(a);
            return Ok(a.ID);
        }

        ///<summary>Creates a new League (currently hard coded)</summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add_smash")]
        public async Task<ActionResult> CreateSmashLeague()
        {
            var smashLeague = new CreateActivityViewModel
            {
                Name = "Super Smash Bros. Ultimate 1v1",
                RegistrationStart = DateTime.Now,
                RegistrationEnd = DateTime.Now,
                SportID = 1,
                MinCapacity = 2,
                MaxCapacity = 16,
                SoloRegistration = true,
                Logo = null,
            };
            await _activityService.PostActivity(smashLeague);
            return Ok();
        }

    }
}
