using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/studentemployment")]
    [Authorize]
    [CustomExceptionFilter]
    public class StudentEmploymentController : ApiController
    {

        private IStudentEmploymentService _studentEmploymentService;


        public StudentEmploymentController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _studentEmploymentService = new StudentEmploymentService(_unitOfWork);

        }
        public StudentEmploymentController(IStudentEmploymentService studentEmploymentService)
        {
            _studentEmploymentService = studentEmploymentService;
        }

        /// <summary>
        /// Get a single membership based on the id given
        /// </summary>
        /// <param name="id">The id of a membership within the database</param>
        /// <remarks>Queries the database about the specified membership</remarks>
        /// <returns>The information about one specific membership</returns>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.STUDENTEMPLOYMENT)]
        public IHttpActionResult Get(int id)
        {
            if(!ModelState.IsValid)
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

            var result =  _studentEmploymentService.Get(id);

            if( result == null)
            {
                return NotFound();
            }

            StudentEmploymentViewModel employment = result;
            return Ok(employment);
        }

    }
}