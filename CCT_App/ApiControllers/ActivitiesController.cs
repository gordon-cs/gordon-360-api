using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;
using System.Data.Entity.Core.Objects;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/activities")]
    public class ActivitiesController : ApiController
    {

        private CCTEntities cct_db_context = new CCTEntities();

        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IEnumerable<ACT_CLUB_DEF> Get()
        {
            return cct_db_context.ACT_CLUB_DEF;
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = cct_db_context.ACT_CLUB_DEF.Find(id);
            return Ok(result);
        }

        //TODO: Logic for finding current session. 
        [HttpGet]
        [Route("{id}/supervisor")]
        public IHttpActionResult GetSupervisorForActivity(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var current_session = cct_db_context.CM_SESSION_MSTR.Max(i => i.SESS_CDE);
            ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result> valid_activity_codes = cct_db_context.ACTIVE_CLUBS_PER_SESS_ID(current_session);
            bool offered = false;
            foreach (ACTIVE_CLUBS_PER_SESS_ID_Result activity in valid_activity_codes)
            {
                if (id.Equals(activity.ACT_CDE.Trim()))
                {
                    offered = true;
                }
            }

            if(!offered)
            {
                return NotFound();
            }

            var supervisor = cct_db_context.SUPERVISORs.Where(s => s.ACT_CDE == id && s.SESSION_CDE == current_session).ToList().ElementAtOrDefault(0);

            return Ok(supervisor);
            
        }

     
    }
}