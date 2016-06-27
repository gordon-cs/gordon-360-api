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

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private CCTEntities database = new CCTEntities();

        public RolesController(CCTEntities dbContext)
        {
            database = dbContext;
        }
        // GET: api/roles
        [HttpGet]
        [Route("")]

        public IHttpActionResult Get()
        {
            var all = database.PART_DEF.ToList();
            return Ok(all);
        }

        // GET: api/PART_DEF/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(PART_DEF))]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var role = database.PART_DEF.Find(id);
            
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        
    }
}