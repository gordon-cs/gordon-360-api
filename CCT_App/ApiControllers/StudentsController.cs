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

        /// <summary>Get a list of all students in the database</summary>
        /// <returns>A list of student names, IDs, and emails</returns>
        /// <remarks>Queries the database for every student record within the student table</remarks>
        // GET: api/Students
        [Route("")]
        [HttpGet]
        public IQueryable<Student> GetStudents()
        {
            return db.Students;
        }

        /// <summary>Get information about a single student</summary>
        /// <param name="id">The Gordon ID of desired student</param>
        /// <returns>The information about the specified student</returns>
        /// <remarks>Queries the database for the specific student identified by their Gordon ID</remarks>
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

        /// <summary>Get any and all memberships that a specific student has been a part of</summary>
        /// <param name="id">The Gordon ID of whichever student memberships are wanted for</param>
        /// <returns>The membership information that the student is a part of</returns>
        /// <remarks>Queries the database for membership information regarding the student id specified only</remarks>
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

        /// <summary>Update an existing student record</summary>
        /// <param name="id">The ID of an existing student</param>
        /// <param name="student">The student object that will be changed</param>
        /// <returns>The updated student object</returns>
        /// <remarks>Queries the database to edit the values of one row</remarks>
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

        /// <summary>Add a new student</summary>
        /// <param name="student">The name of the new student</param>
        /// <returns>The student information that was added</returns>
        /// <remarks>Queries the database to add a new student</remarks>
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

        /// <summary>Delete an existing student record</summary>
        /// <param name="id">The student ID for the student being removed</param>
        /// <returns>The student information that was removed</returns>
        /// <remarks>Queries the database to remove the row in the students table with the same ID as the one from the URL string</remarks>
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