using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CCT_App.Models;
using CCT_App.Repositories;
using CCT_App.Services;
using System.Data.Entity.Core.Objects;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("KJzKJ6FOKx/api/sessions")]
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
        [ResponseType(typeof(CM_SESSION_MSTR))]
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

        /// <summary>Gets the activities taking place during a given session</summary>
        /// <param name="id">The session identifier</param>
        /// <returns>A list of all activities that are active during the given session determined by the id parameter</returns>
        /// <remarks>Queries the database to find which activities are active during the session desired</remarks>
        // GET: api/sessions/id/activities
        [HttpGet]
        [Route("{id}/activities")]
        public IHttpActionResult GetActivitiesForSession(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            var result = _sessionService.GetActivitiesForSession(id);

            if(result == null)
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
