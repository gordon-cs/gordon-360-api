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
using System.Data.Entity.Core.Objects;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/sessions")]
    public class SessionsController : ApiController
    {

        private CCTEntities database = new CCTEntities();

        public SessionsController(CCTEntities dbContext)
        {
            database = dbContext;
        }
        // GET: api/Sessions
        [HttpGet]
        [Route("")]
        public IQueryable<CM_SESSION_MSTR> GetCM_SESSION_MSTR()
        {
            return database.CM_SESSION_MSTR;
        }

        // GET: api/Sessions/5
        [ResponseType(typeof(CM_SESSION_MSTR))]
        public IHttpActionResult GetCM_SESSION_MSTR(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = database.CM_SESSION_MSTR.FirstOrDefault(s => s.SESS_CDE.Trim() == id);

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
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result> valid_activity_codes = database.ACTIVE_CLUBS_PER_SESS_ID(id);

            return Ok(valid_activity_codes);

        }
     
    }
}