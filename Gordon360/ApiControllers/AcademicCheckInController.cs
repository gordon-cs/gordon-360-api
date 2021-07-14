using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/checkIn")]
    [Authorize]
    [CustomExceptionFilter]
    public class AcademicCheckInController : ApiController 
    {
        private IAcademicCheckInService _checkInService;
        private IAccountService _accountService;
                
        public AcademicCheckInController() 
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _checkInService = new AcademicCheckInService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        /*
        /// <summary>Gets a student's holds by id from the database</summary>
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
        */
        /*
        public IHttpActionResult getDemographics()
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
        */




        /// <summary>Set emergency contacts for student</summary>
        /// <param name="data"> The contact data to be stored </param>
        /// <returns> The data stored </returns>
        [HttpPost]
        [Route("emergencycontact")]
        public IHttpActionResult PutEmergencyContact([FromBody] EmergencyContact data)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = _checkInService.PutEmergencyContact(data, id);
                return Created("Emergency Contact", result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error setting the check in data.");
                return NotFound();
            }
        
        }



        /// <summary> Sets the students cell phone number</summary>
        /// <param name="data"> The phone number object to be added to the database </param>
        /// <returns> The data stored </returns>
        [HttpPut]
        [Route("cellphone")]
        public IHttpActionResult PutCellPhone([FromBody] AcademicCheckInViewModel data)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = _checkInService.PutCellPhone(id, data);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error setting the check in data.");
                return NotFound();
            }
        
        }

        /// <summary>Sets the students race and ethinicity</summary>
        /// <param name="data"> The object containing the race numbers of the users </param>
        /// <returns> The data stored </returns>
        [HttpPut]
        [Route("demographic")]
        public IHttpActionResult PutDemographic([FromBody] AcademicCheckInViewModel data)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try
            {
                var result = _checkInService.PutDemographic(id, data);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error setting the check in data.");
                return NotFound();
            }

        }

    }
}
