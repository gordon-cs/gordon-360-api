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
        public IQueryable<PART_DEF> GetRoles()
        {
            return database.PART_DEF;
        }

        // GET: api/PART_DEF/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(PART_DEF))]
        public IHttpActionResult GetRoles(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = database.PART_DEF.FirstOrDefault(r => r.PART_CDE.Trim() == id);
            
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        
    }
}