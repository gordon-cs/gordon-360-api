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
using Newtonsoft.Json.Linq;
using Gordon360.Static.Names;
using System.Threading.Tasks;
using Gordon360.Models;
using System.Diagnostics;
using Gordon360.Providers;
using System.IO;
using Gordon360.Models.ViewModels;
using System.Security.Claims;
using Gordon360.AuthorizationFilters;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/checkIn")]
    [Authorize]
    [CustomExceptionFilter]
    public class AcademicCheckInController : ApiController 
    {
        private IAcadmicCheckInService _checkInService;
                
        public AcademicCheckInController() 
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _checkInService = new AcademicCheckInService(_unitOfWork);
        }
        
        /// <summary>Gets a student's holds by id from the database</summary>
        /// <param name="studentID">The id of the student to retrieve the holds of</param>
        /// <returns>The student's current holds (if any)</returns>
        [HttpGet]
        [Route("holds")]
        // TO DO: Add StateYourBusiness??
        // Private route to authenticated users
        public IHttpActionResult GetHolds()
        {
            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _checkInService.GetHolds(id);

            if (result == null){
                return NotFound();
            }
            return Ok(result);
        }

        public IHTTPActionResult getDemographics()
        {
            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _checkInService.getDemographics(id);

            if (result == null){
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Set emergency contacts for student
        /// <param name="data"> The contact data to be stored
        /// </summary>
        /// <returns> The data stored </returns>
        [HttpPost]
        [Route("emergencycontact")]
        public IHTTPActionResult PutEmergencyContact(var data)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = _checkInService.PutEmergencyContact(data, id);
                return result;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error setting the check in data.");
            }
        
        }

        /// <summary>
        /// Sets the students cell phone number
        /// <param name="data"> The phone number object to be added to the database
        /// </summary>
        /// <returns> The data stored </returns>
        [HttpPut]
        [Route("cellphone")]
        public IHTTPActionResult PutCellPhone(var data)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = _checkInService.PutCellPhone(data, id);
                return result;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error setting the check in data.");
            }
        
        }
    }
}
