using System;
using System.Web.Http;
using System.Web.Http.Description;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.AuthorizationFilters;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/students")]
    [Authorize]
    public class StudentsController : ApiController
    {
        private IStudentService _studentService;
        
        public StudentsController()
        {
            var _unitOfWork = new UnitOfWork();
            _studentService = new StudentService(_unitOfWork);
        }
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>Get a list of all students in the database</summary>
        /// <returns>A list of student names, IDs, and emails</returns>
        /// <remarks>Queries the database for every student record within the student table</remarks>
        // GET: api/Students
        [Route("")]
        [HttpGet]
        [AuthorizationLevel(authorizationLevel = Constants.GOD_LEVEL)]
        public IHttpActionResult Get()
        {
            var all = _studentService.GetAll();
            return Ok(all);
        }

        /// <summary>Get information about a single student</summary>
        /// <param name="id">The Gordon ID of desired student</param>
        /// <returns>The information about the specified student</returns>
        /// <remarks>Queries the database for the specific student identified by their Gordon ID</remarks>
        // GET: api/Students/5
        [Route("{id}")]
        [HttpGet]
        [AuthorizationLevel(authorizationLevel = Constants.GOD_LEVEL)]
        public IHttpActionResult Get(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var result = _studentService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        // TODO FIGGURE THIS OUT
        /// <summary>Get any and all memberships that a specific student has been a part of</summary>
        /// <param name="id">The Gordon ID of whichever student memberships are wanted for</param>
        /// <returns>The membership information that the student is a part of</returns>
        /// <remarks>Queries the database for membership information regarding the student id specified only</remarks>
        [ResponseType(typeof(IHttpActionResult))]
        [Route("{id}/memberships")]
        [HttpGet]
        public IHttpActionResult GetActivitiesForStudent(string id)
        {

            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _studentService.GetActivitiesForStudent(id);
            
            if( result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


    }
}
