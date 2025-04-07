using Gordon360.Authorization;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers.Api;

[Route("api/checkIn")]
[Authorize]
[CustomExceptionFilter]
public class AcademicCheckInController(IAcademicCheckInService academicCheckInService, IAccountService accountService) : ControllerBase
{

    /// <summary>Set emergency contacts for student</summary>
    /// <param name="data"> The contact data to be stored </param>
    /// <returns> The data stored </returns>
    [HttpPost]
    [Route("emergencycontact")]
    public async Task<ActionResult<EmergencyContactViewModel>> PutEmergencyContactAsync([FromBody] EmergencyContactViewModel data)
    {
        var username = AuthUtils.GetUsername(User);
        var id = accountService.GetAccountByUsername(username).GordonID;

        try
        {
            var result = await academicCheckInService.PutEmergencyContactAsync(data, id, username);
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
    [HttpPut]
    [Route("cellphone")]
    public async Task<ActionResult> PutCellPhoneAsync([FromBody] MobilePhoneUpdateViewModel data)
    {
        var username = AuthUtils.GetUsername(User);

        try
        {
            await academicCheckInService.PutCellPhoneAsync(username, data);
            return Ok();
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
        var id = accountService.GetAccountByUsername(username).GordonID;

        try
        {
            var result = await academicCheckInService.PutDemographicAsync(id, data);
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
    public async Task<ActionResult<EnrollmentCheckinHolds>> GetHoldsAsync()
    {
        var username = AuthUtils.GetUsername(User);
        var id = accountService.GetAccountByUsername(username).GordonID;

        try
        {
            EnrollmentCheckinHolds holds = await academicCheckInService.GetHoldsAsync(id);
            return Ok(holds);
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
        var id = accountService.GetAccountByUsername(username).GordonID;

        try
        {
            await academicCheckInService.SetStatusAsync(id);
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
        var id = accountService.GetAccountByUsername(username).GordonID;

        try
        {
            var result = await academicCheckInService.GetStatusAsync(id);
            return Ok(result);
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return NotFound();
        }
    }

}
