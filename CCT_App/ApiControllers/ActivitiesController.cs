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
    [RoutePrefix("api/activities")]
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

        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _activityService.GetAll();
            return Ok(all);
        }

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

        //TODO: Logic for finding current session. 
        [HttpGet]
        [Route("{id}/supervisor")]
        public IHttpActionResult GetSupervisorForActivity(string id)
        {

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _activityService.GetSupervisorForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

     
    }
}