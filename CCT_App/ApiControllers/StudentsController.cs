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
        private CCTEntities db = new CCTEntities();

        // GET: api/Students
        [Route("")]
        [HttpGet]
        public IQueryable<Student> GetStudents()
        {
            return db.Students;
        }

        // GET: api/Students/5
        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(Student))]
        public IHttpActionResult GetStudent(string id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [Route("{id}/memberships")]
        [HttpGet]
        public IHttpActionResult GetActivitiesForStudent(string id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Membership> memberships = null;

            try
            {
                memberships = db.Memberships.Where(mem => mem.ID_NUM == id).ToList();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(memberships);

        }

        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStudent(string id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.student_id)
            {
                return BadRequest();
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        [ResponseType(typeof(Student))]
        public IHttpActionResult PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Students.Add(student);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.student_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = student.student_id }, student);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        public IHttpActionResult DeleteStudent(string id)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            db.SaveChanges();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(string id)
        {
            return db.Students.Count(e => e.student_id == id) > 0;
        }
    }
}