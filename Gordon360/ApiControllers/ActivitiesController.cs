using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;

namespace Gordon360.Controllers.Api
{
    
    [RoutePrefix("api/activities")]
    [Authorize]
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
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP)]
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
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP)]
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
        /// Gets the memberships leaders associated with a given activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/leaders")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP)]
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