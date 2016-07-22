using System;
using System.Web.Http;
using System.Web.Http.Description;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System.Diagnostics;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/students")]
    [Authorize]
    [CustomExceptionFilter]
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
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.STUDENT)]
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
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT)]
        public IHttpActionResult Get(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _studentService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Get information about a single student</summary>
        /// <param name="email">The email of desired student</param>
        /// <returns>The information about the specified student</returns>
        /// <remarks>Queries the database for the specific student identified by their email</remarks>
        // GET: api/Students/email/john.doe@gordon.edu
        [Route("email/{email}")]
        [HttpGet]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT)]
        public IHttpActionResult GetByEmail(string email)
        {
  
            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(email))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _studentService.GetByEmail(email);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


    }
}
