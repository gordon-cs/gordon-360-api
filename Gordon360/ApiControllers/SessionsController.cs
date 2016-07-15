using System;
using System.Web.Http;
using System.Web.Http.Description;
using Gordon360.Models;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/sessions")]
    [Authorize]
    public class SessionsController : ApiController
    {

        private ISessionService _sessionService;


        public SessionsController()
        {
            var _unitOfWork = new UnitOfWork();
            _sessionService = new SessionService(_unitOfWork);
        }
        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>Get a list of all sessions</summary>
        /// <returns>All sessions within the database</returns>
        /// <remarks>Queries the database for all sessions, current and past</remarks>
        // GET: api/Sessions
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _sessionService.GetAll();
            return Ok(all);
        }

        /// <summary>Get one specific session specified by the id in the URL string</summary>
        /// <param name="id">The identifier for one specific session</param>
        /// <returns>The information about one specific session</returns>
        /// <remarks>Queries the database regarding a specific session with the given identifier</remarks>
        // GET: api/Sessions/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var result = _sessionService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the current active session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public IHttpActionResult GetCurrentSession()
        {
            var currentSession = Helpers.GetCurrentSession();
            if(currentSession == null)
            {
                return NotFound();
            }

            return Ok(currentSession);
        }

    }
}
