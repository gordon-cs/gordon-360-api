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

        /// <summary>Get all supervisors</summary>
        /// <returns>All supervisors and their corresponding information</returns>
        /// <remarks>Queries the database for all supervisors</remarks>
        // GET: api/Supervisors
        [HttpGet]
        [Route("")]
        public IQueryable<SUPERVISOR> GetSUPERVISORs()
        {
            return db.SUPERVISORs;
        }

        /// <summary>Get a single supervisor</summary>
        /// <param name="id">The ID of desired supervisor</param>
        /// <returns>The supervisor object that has an ID matching the one specified in the URL</returns>
        /// <remarks>Queries the database for a specific supervisor based on their Gordon ID</remarks>
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

        /// <summary>Update an existing supervisor</summary>
        /// <param name="id">The id for an existing supervisor</param>
        /// <param name="supervisor">The supervisor object to be changed</param>
        /// <returns>The changed supervisor object</returns>
        /// <remarks>Queries the database to update one supervisor</remarks>
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

        /// <summary>Add a new supervisor</summary>
        /// <param name="sUPERVISOR">The name of the new supervisor</param>
        /// <returns>The new supervisor object</returns>
        /// <remarks>Queries the database to add a new supervisor into the table</remarks>
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

        /// <summary>Delete a supervisor</summary>
        /// <param name="id">The ID of supervisor to be deleted</param>
        /// <returns>The supervisor object that was deleted</returns>
        /// <remarks>Queries the database to remove the row of the specified supervisor</remarks>
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