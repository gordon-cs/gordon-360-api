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
    [RoutePrefix("api/supervisors")]
    public class SupervisorsController : ApiController
    {
        private CCTEntities database = new CCTEntities();


        public SupervisorsController(CCTEntities dbContext)
        {
            database = dbContext;
        }
        // GET: api/Supervisors
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = database.SUPERVISORs.ToList();
            return Ok(all);
        }

        // GET: api/Supervisors/5
        [ResponseType(typeof(SUPERVISOR))]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            SUPERVISOR supervisor = database.SUPERVISORs.Find(id);

            if (supervisor == null)
            {
                return NotFound();
            }

            return Ok(supervisor);
        }

        // PUT: api/Supervisors/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] SUPERVISOR supervisor)
        {
            if (!ModelState.IsValid || supervisor == null)
            {
                return BadRequest(ModelState);
            }

            if (id != supervisor.SUP_ID)
            {
                return BadRequest();
            }

            database.Entry(supervisor).State = EntityState.Modified;

            try
            {
                database.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SUPERVISORExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Supervisors
        [ResponseType(typeof(SUPERVISOR))]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(SUPERVISOR supervisor)
        {
            if (!ModelState.IsValid || supervisor == null)
            {
                return BadRequest();
            }

            ACCOUNT person = database.ACCOUNTs.Find(supervisor.ID_NUM);

            if (person == null)
            {
                return NotFound();
            }

            CM_SESSION_MSTR session = database.CM_SESSION_MSTR.Find(supervisor.SESSION_CDE);

            if (session == null)
            {
                return NotFound();
            }
            var potential_actvities = database.ACTIVE_CLUBS_PER_SESS_ID(supervisor.SESSION_CDE);

            bool offered = false;

            foreach( ACTIVE_CLUBS_PER_SESS_ID_Result activity in potential_actvities)
            {
                if(activity.ACT_CDE == supervisor.ACT_CDE)
                {
                    offered = true;
                }
            }

            if (!offered)
            {
                return NotFound();
            }

            database.SUPERVISORs.Add(supervisor);

            database.SaveChanges();

            return Created("DefaultApi",  supervisor);
        }

        // DELETE: api/Supervisors/5
        [ResponseType(typeof(SUPERVISOR))]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            SUPERVISOR supervisor = database.SUPERVISORs.Find(id);

            if (supervisor == null)
            {
                return NotFound();
            }

            database.SUPERVISORs.Remove(supervisor);
            database.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SUPERVISORExists(int id)
        {
            return database.SUPERVISORs.Count(e => e.SUP_ID == id) > 0;
        }
    }
}