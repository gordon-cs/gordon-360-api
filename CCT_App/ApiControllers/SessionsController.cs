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
    [RoutePrefix("api/sessions")]
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

        // GET: api/Sessions
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _sessionService.GetAll();
            return Ok(all);
        }

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
     
    }
}