using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;

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
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = cct_db_context.ACT_CLUB_DEF.Find(id);
            return Ok(result);
        }

     
    }
}