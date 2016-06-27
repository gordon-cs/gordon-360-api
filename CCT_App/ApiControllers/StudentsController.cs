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
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        private CCTEntities database = new CCTEntities();

        public StudentsController(CCTEntities dbContext)
        {
            database = dbContext;
        }

        // GET: api/Students
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Student>))]
        public IEnumerable<Student> Get()
        {
            return database.Students;
        }

        // GET: api/Students/5
        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult Get(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }
            var student = database.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }
        
        [ResponseType(typeof(IHttpActionResult))]
        [Route("{id}/memberships")]
        [HttpGet]
        public IHttpActionResult GetActivitiesForStudent(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            List<Membership> memberships = new List<Membership>();

         
            memberships = database.Memberships.Where(mem => mem.ID_NUM == id).ToList();
         

            return Ok(memberships);

        }

        // PUT: api/Students/5
        [ResponseType(typeof(IHttpActionResult))]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(string id, [FromBody] Student student)
        {
            if (!ModelState.IsValid || student == null || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            if (id != student.student_id)
            {
                return BadRequest();
            }

            var original = database.Students.Find(id);

            if (original == null)
            {
                return NotFound();
            }

            database.Entry(student).State = EntityState.Modified;
            database.SaveChanges();
           
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Students
        [ResponseType(typeof(IHttpActionResult))]
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostStudent(Student student)
        {
            if (!ModelState.IsValid || student == null)
            {
                return BadRequest(ModelState);
            }

            database.Students.Add(student);

            return CreatedAtRoute("DefaultApi", new { id = student.student_id }, student);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        public IHttpActionResult DeleteStudent(string id)
        {
            Student student = database.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            database.Students.Remove(student);
            database.SaveChanges();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }

  
    }
}