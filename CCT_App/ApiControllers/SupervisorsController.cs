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

namespace CCT_App.ApiControllers
{
    [RoutePrefix("api/supervisors")]
    public class SupervisorsController : ApiController
    {
        private CCTEntities db = new CCTEntities();

        // GET: api/Supervisors
        [HttpGet]
        [Route("")]
        public IQueryable<SUPERVISOR> GetSUPERVISORs()
        {
            return db.SUPERVISORs;
        }

        // GET: api/Supervisors/5
        [ResponseType(typeof(SUPERVISOR))]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetSUPERVISOR(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SUPERVISOR sUPERVISOR = db.SUPERVISORs.Find(id);

            if (sUPERVISOR == null)
            {
                return NotFound();
            }

            return Ok(sUPERVISOR);
        }

        // PUT: api/Supervisors/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult PutSUPERVISOR(int id, [FromBody] SUPERVISOR supervisor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supervisor.SUP_ID)
            {
                return BadRequest();
            }

            db.Entry(supervisor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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
        public IHttpActionResult PostSUPERVISOR(SUPERVISOR sUPERVISOR)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SUPERVISORs.Add(sUPERVISOR);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = sUPERVISOR.SUP_ID }, sUPERVISOR);
        }

        // DELETE: api/Supervisors/5
        [ResponseType(typeof(SUPERVISOR))]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteSUPERVISOR(int id)
        {
            SUPERVISOR sUPERVISOR = db.SUPERVISORs.Find(id);
            if (sUPERVISOR == null)
            {
                return NotFound();
            }

            db.SUPERVISORs.Remove(sUPERVISOR);
            db.SaveChanges();

            return Ok(sUPERVISOR);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SUPERVISORExists(int id)
        {
            return db.SUPERVISORs.Count(e => e.SUP_ID == id) > 0;
        }
    }
}