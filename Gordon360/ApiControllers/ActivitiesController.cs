using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;

namespace Gordon360.Controllers.Api
{
    
    [RoutePrefix("api/activities")]
    [CustomExceptionFilter]
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
        /// <returns>All the activities in the databse</returns>
        /// <remarks></remarks>
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

        /// <summary>Gets the activities taking place during a given session</summary>
        /// <param name="id">The session identifier</param>
        /// <returns>A list of all activities that are active during the given session determined by the id parameter</returns>
        /// <remarks>Queries the database to find which activities are active during the session desired</remarks>
        // GET: api/sessions/id/activities
        [HttpGet]
        [Route("session/{id}")]
        public IHttpActionResult GetActivitiesForSession(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            var result = _activityService.GetActivitiesForSession(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

    }
}