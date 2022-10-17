using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class AdminsController : GordonControllerBase
    {
        private readonly IAdministratorService _adminService;

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
        public ActionResult<IEnumerable<AdminViewModel?>> GetAll()
        {
            var result = _adminService.GetAll();
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
        public ActionResult<AdminViewModel> Post([FromBody] AdminViewModel admin)
        {
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
        public ActionResult<AdminViewModel> Delete(int id)
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
