using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gordon360.Controllers.Api
{
    [Route("api/checkIn")]
    [Authorize]
    [CustomExceptionFilter]
    public class AcademicCheckInController : ControllerBase 
    {
        private readonly IAcademicCheckInService _checkInService;
        private readonly IAccountService _accountService;

        public AcademicCheckInController(CCTContext context)
        {
            _checkInService = new AcademicCheckInService(context);
            _accountService = new AccountService(context);
        }

        /// <summary>Set emergency contacts for student</summary>
        /// <param name="data"> The contact data to be stored </param>
        /// <returns> The data stored </returns>
        [HttpPost]
        [Route("emergencycontact")]
        public async Task<ActionResult<EmergencyContactViewModel>> PutEmergencyContactAsync([FromBody] EmergencyContactViewModel data)
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = await _checkInService.PutEmergencyContactAsync(data, id, username);
                return Created("Emergency Contact", result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }
        
        }



        /// <summary> Sets the students cell phone number</summary>
        /// <param name="data"> The phone number object to be added to the database </param>
        /// <returns> The data stored </returns>
        [HttpPut]
        [Route("cellphone")]
        public async Task<ActionResult<AcademicCheckInViewModel>> PutCellPhoneAsync([FromBody] AcademicCheckInViewModel data)
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try {
                var result = await _checkInService.PutCellPhoneAsync(id, data);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }
        
        }

        /// <summary>Sets the students race and ethinicity</summary>
        /// <param name="data"> The object containing the race numbers of the users </param>
        /// <returns> The data stored </returns>
        [HttpPut]
        [Route("demographic")]
        public async Task<ActionResult<AcademicCheckInViewModel>> PutDemographicAsync([FromBody] AcademicCheckInViewModel data)
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try
            {
                var result = await _checkInService.PutDemographicAsync(id, data);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }

        }

        /// <summary> Gets and returns the user's holds </summary>
        /// <returns> The user's stored holds </returns>
        [HttpGet]
        [Route("holds")]
        public async Task<ActionResult<AcademicCheckInViewModel>> GetHoldsAsync()
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try
            {
                var result = (await _checkInService.GetHoldsAsync(id)).First();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }

        }

        /// <summary> Sets the user as having completed Academic Checkin </summary>
        /// <returns> The HTTP status indicating whether the request was completed or not</returns>
        [HttpPut]
        [Route("status")]
        public async Task<ActionResult> SetStatusAsync()
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try
            {
                await _checkInService.SetStatusAsync(id);
                return Ok();
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }
        }

        /// <summary> Gets whether the user has checked in or not. True if they have checked in, false if they have not checked in </summary>
        /// <returns> The HTTP status indicating whether the request was completed and returns the check in status of the student </returns>
        [HttpGet]
        [Route("status")]
        public async Task<ActionResult<AcademicCheckInViewModel>> GetStatusAsync()
        {
            var username = AuthUtils.GetUsername(User);
            var id = _accountService.GetAccountByUsername(username).GordonID;

            try
            {
                var result = await _checkInService.GetStatusAsync(id);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return NotFound();
            }
        }

    }
}
