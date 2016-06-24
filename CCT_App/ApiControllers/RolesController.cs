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
        private CCTEntities db = new CCTEntities();

        /// <summary>Get all the roles a person may have within an activity</summary>
        /// <returns>A list of all the roles and their coresponding acronyms</returns>
        /// <remarks>Queries the database for all the roles that are valid</remarks>
        // GET: api/roles
        [HttpGet]
        [Route("")]
        public IQueryable<PART_DEF> GetRoles()
        {
            return db.PART_DEF;
        }

        /// <summary>Get a single role and the information about it</summary>
        /// <param name="id">The identifier for a single role</param>
        /// <returns>The information about the specified role</returns>
        /// <remarks>Queries the database and returns information about one particular role</remarks>
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
            var role = db.PART_DEF.FirstOrDefault(r => r.PART_CDE.Trim() == id);
            
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        
    }
}