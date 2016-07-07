using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models.ViewModels;
using CCT_App.Services;
using CCT_App.Repositories;

namespace CCT_App.ApiControllers
{
    // TODO: GET RID OF THIS CLASS
    [RoutePrefix("activitiesInfo")]
    public class ActivityInfoController : ApiController
    {
        private IActivityInfoService _activityInfoService;

        public ActivityInfoController()
        {
            var _unitOfWork = new UnitOfWork();
            _activityInfoService = new ActivityInfoService(_unitOfWork);
        }

        public ActivityInfoController(IActivityInfoService activityInfoService)
        {
            _activityInfoService = activityInfoService;
        }

        /// <summary>
        /// Get all available information for activities
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Server makes a call to get a list of all activity information from the database
        /// </remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _activityInfoService.GetAll();
            return Ok(all);
        }

        /// <summary>Get the activityInfo object whose activity code corresponds to the given parameter</summary>
        /// <param name="id">The activity code</param>
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
            var result = _activityInfoService.Get(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
