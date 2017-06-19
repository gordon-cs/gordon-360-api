using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;



namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/profiles")]
    [CustomExceptionFilter]
    [Authorize]
    public class ProfilesController : ApiController
    {
        private IProfileService _profileService;

        public ProfilesController()
        {
            var _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
        }

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        ///// <summary>
        ///// Get all available activities
        ///// </summary>
        ///// <returns>All the activities in the databse</returns>
        ///// <remarks></remarks>
        //// GET api/<controller>
        //[HttpGet]
        //[Route("")]
        //public IHttpActionResult Get()
        //{
        //    var all = _profileService.GetAll();
        //    return Ok(all);
        //}

        /// <summary>Get a single activity based upon the string id entered in the URL</summary>
        /// <param name="username">An identifier for a single activity</param>
        /// <returns></returns>
        /// <remarks>Get a single activity from the database</remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
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
            var student = _profileService.GetStudentProfileByUsername(username);
            var faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var alumni = _profileService.GetAlumniProfileByUsername(username);
            if (student != null)
            {
                return Ok(student);
            }
            else if (faculty != null)
            {
                return Ok(faculty);                
            }
            else if (alumni != null)
            {
                return Ok(alumni);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
