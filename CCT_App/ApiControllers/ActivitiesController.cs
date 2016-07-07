using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;
using System.Data.Entity.Core.Objects;
using CCT_App.Services;
using CCT_App.Repositories;

namespace CCT_App.Controllers.Api
{
    
    [RoutePrefix("KJzKJ6FOKx/api/activities")]
    public class ActivitiesController : ApiController
    {
        private IActivityService _activityService;
        
        public ActivitiesController()
        {
            var _unitOfWork = new UnitOfWork();
            _activityService = new ActivityService(_unitOfWork);
        }

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Get all available activities
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Server makes a call to get a lsit of all activities from the database
        /// </remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _activityService.GetAll();
            return Ok(all);
        }

        /// <summary>Get a single activity based upon the string id entered in the URL</summary>
        /// <param name="id">An identifier for a single activity</param>
        /// <returns></returns>
        /// <remarks>Get a single activity from the database</remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _activityService.Get(id);

            if ( result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get the supervisors for a given activity
        /// </summary>
        /// <param name="id">The identifier for a specific activity</param>
        /// <returns></returns>
        /// <remarks>
        /// Get the supervisors for a specified activity within the database
        /// </remarks>
        [HttpGet]
        [Route("{id}/supervisor")]
        public IHttpActionResult GetSupervisorsForActivity(string id)
        {

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _activityService.GetSupervisorsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>
        /// Get all the memberships associated with a given activity
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("{id}/memberships")]
        public IHttpActionResult GetMembershipsForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _activityService.GetMembershipsForActivity(id);

            if (result == null )
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the leaders associated with a given activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/leaders")]
        public IHttpActionResult GetLeadersForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _activityService.GetLeadersForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
     
    }
}