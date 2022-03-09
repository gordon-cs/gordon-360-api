using System.Collections.Generic;
using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models;
using Gordon360.Models.CCT;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers
{
    [Route("api/admins")]
    [Authorize]
    [CustomExceptionFilter]
    public class AdminsController : ControllerBase
    {

        private readonly IAdministratorService _adminService;

        public AdminsController(CCTContext context)
        {
            _adminService = new AdministratorService(context);
        }
        public AdminsController(IAdministratorService adminService)
        {
            _adminService = adminService;
        }


        /// <summary>
        /// Get all admins
        /// </summary>
        /// <returns>
        /// A list of all admins
        /// </returns>
        /// <remarks>
        /// Server makes call to the database and returns all admins
        /// </remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ADMIN)]
        public ActionResult<IEnumerable<ADMIN>> GetAll()
        {
            var result = _adminService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Get a specific admin
        /// </summary>
        /// <returns>
        /// The specific admin
        /// </returns>
        /// <remarks>
        /// Server makes call to the database and returns the specific admin
        /// </remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ADMIN)]
        public ActionResult<ADMIN> GetByGordonId(string id)
        {
            var result = _adminService.Get(id);
            return Ok(result);
        }

        /// <summary>Create a new admin to be added to database</summary>
        /// <param name="admin">The admin item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new admin to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name = "Admins")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.ADMIN)]
        public ActionResult<ADMIN> Post([FromBody] ADMIN admin)
        {
            if (!ModelState.IsValid || admin == null)
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

            var result = _adminService.Add(admin);

            if (result == null)
            {
                return NotFound();
            }

            return Created("admins", admin);
        }

        /// <summary>Delete an existing admin</summary>
        /// <param name="id">The identifier for the admin to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given admin from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.ADMIN)]
        public ActionResult<ADMIN> Delete(int id)
        {
            var result = _adminService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
