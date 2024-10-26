using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.Housing;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System;
using Gordon360.Exceptions;


namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class HousingController(CCTContext context, IProfileService profileService, IHousingService housingService) : GordonControllerBase
{

    /// <summary>
    /// Delete an application (and consequently all rows that reference it)
    /// </summary>
    /// <param name="applicationID"> The id of the application to remove </param>
    /// <returns></returns>
    [HttpDelete]
    [Route("apartment/applications/{applicationID}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.HOUSING)]
    public ActionResult<bool> DeleteApplication(int applicationID)
    {
        bool result = housingService.DeleteApplication(applicationID);
        return Ok(result);
    }


    /// <summary>
    /// Get a list of the apartment-style halls
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("halls/apartments")]
    public ActionResult<string[]> GetApartmentHalls()
    {
        var result = housingService.GetAllApartmentHalls();
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get apartment application ID number of currently logged in user if that user is on an existing application
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("apartment")]
    public ActionResult<int?> GetApplicationID()
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        string sessionID = Helpers.GetCurrentSession(context);

        int? result = housingService.GetApplicationID(authenticatedUserUsername, sessionID);
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get apartment application ID number for a user if that user is on an existing application
    /// </summary>
    /// <param name="username">username of the profile info</param>
    /// <returns></returns>
    [HttpGet]
    [Route("apartment/{username}")]
    public ActionResult<int?> GetUserApplicationID(string username)
    {
        string sessionID = Helpers.GetCurrentSession(context);

        int? result = housingService.GetApplicationID(username, sessionID);
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// save application
    /// </summary>
    /// <returns>Returns the application ID number if all the queries succeeded</returns>
    [HttpPost]
    [Route("apartment/applications")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING)]
    public ActionResult<int> SaveApplication([FromBody] ApartmentApplicationViewModel applicationDetails)
    {
        string sessionID = Helpers.GetCurrentSession(context);

        string editorUsername = applicationDetails.EditorProfile?.AD_Username ?? applicationDetails.EditorUsername;

        var apartmentApplicants = applicationDetails.Applicants;
        foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
        {
            if (applicant.Profile == null)
            {
                applicant.Profile = profileService.GetStudentProfileByUsername(applicant.Username);
            }
        }

        int result = housingService.SaveApplication(sessionID, editorUsername, apartmentApplicants, applicationDetails.ApartmentChoices);

        return Created("Status of application saving: ", result);
    }

    /// <summary>
    /// update existing application (Differentiated by HttpPut instead of HttpPost)
    /// </summary>
    /// <returns>Returns the application ID number if all the queries succeeded</returns>
    [HttpPut]
    [Route("apartment/applications/{applicationID}")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
    public ActionResult<int> EditApplication(int applicationID, [FromBody] ApartmentApplicationViewModel applicationDetails)
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        string sessionID = Helpers.GetCurrentSession(context);

        string newEditorUsername = applicationDetails.EditorProfile?.AD_Username ?? applicationDetails.EditorUsername;

        var newApartmentApplicants = applicationDetails.Applicants;
        foreach (ApartmentApplicantViewModel applicant in newApartmentApplicants)
        {
            if (applicant.Profile == null)
            {
                applicant.Profile = profileService.GetStudentProfileByUsername(applicant.Username);
            }
        }

        int result = housingService.EditApplication(authenticatedUserUsername, sessionID, applicationID, newEditorUsername, newApartmentApplicants, applicationDetails.ApartmentChoices);

        return Created("Status of application saving: ", result);
    }

    /// <summary>
    /// change the editor (i.e. primary applicant) of the application
    /// </summary>
    /// <returns>The result of changing the editor</returns>
    [HttpPut]
    [Route("apartment/applications/{applicationID}/editor")]
    public ActionResult<bool> ChangeEditor(int applicationID, [FromBody] ApartmentApplicationViewModel applicationDetails)
    {
        //get token data from context, username is the username of current logged in person
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        string newEditorUsername = applicationDetails.EditorProfile?.AD_Username ?? applicationDetails.EditorUsername;

        bool result = housingService.ChangeApplicationEditor(authenticatedUserUsername, applicationID, newEditorUsername);

        return Ok(result);
    }

    /// <summary>
    /// change the date an application was submitted
    /// (changes it from null the first time they submit)
    /// </summary>
    /// <returns>The result of changing the date submitted</returns>
    [HttpPut]
    [Route("apartment/applications/{applicationID}/submit")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
    public ActionResult<bool> ChangeApplicationDateSubmitted(int applicationID)
    {
        bool result = housingService.ChangeApplicationDateSubmitted(applicationID);
        return Ok(result);
    }

    /// <summary>Get apartment application info for a given application ID number</summary>
    /// <param name="applicationID">application ID number of the apartment application</param>
    /// <returns>Object of type ApartmentAppViewModel</returns>
    [HttpGet]
    [Route("apartment/applications/{applicationID}")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)]
    public ActionResult<ApartmentApplicationViewModel> GetApartmentApplication(int applicationID)
    {
        //get token data from context, username is the username of current logged in person
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        var authGroups = AuthUtils.GetGroups(User);

        var isHousingAdmin = housingService.CheckIfHousingAdmin(authenticatedUserUsername);
        bool isAdmin = authGroups.Contains(AuthGroup.SiteAdmin) || isHousingAdmin;

        ApartmentApplicationViewModel result = housingService.GetApartmentApplication(applicationID, isAdmin);
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>Get apartment application info for all applications if the current user is a housing admin</summary>
    /// <returns>Object of type ApartmentApplicationViewModel</returns>
    [HttpGet]
    [Route("admin/apartment/applications/")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.HOUSING)]
    public ActionResult<ApartmentApplicationViewModel[]> GetAllApartmentApplication()
    {
        //get token data from context, username is the username of current logged in person
        var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        
        ApartmentApplicationViewModel[] result = housingService.GetAllApartmentApplication();
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new hall assignment range if it does not overlap with any existing ranges
    /// </summary>
    /// <param name="model">The ViewModel that contains the hall ID and room range</param>
    /// <returns>The created Hall_Assignment_Ranges object</returns>
    [HttpPost("roomrange")]
    public async Task<ActionResult<Hall_Assignment_Ranges>> CreateRoomRange([FromBody] HallAssignmentRangeViewModel model)
    {
        try
        {
            var newRange = await housingService.CreateRoomRangeAsync(model);
            return Created("Room range created successfully.", newRange);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a Room Range
    /// </summary>
    /// <param name="rangeId">The ID of the room range to delete</param>
    /// <returns> Returns if completed</returns>
    [HttpDelete("roomrange/{rangeId}")]
    public async Task<IActionResult> DeleteRoomRange(int rangeId)
    {
        try
        {
            var success = await housingService.DeleteRoomRangeAsync(rangeId);
            if (success)
            {
                return Ok(new { message = "Room range deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to delete the room range." });
            }
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new { message = ex.ExceptionMessage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Assigns an RA to a room range if no RA is currently assigned
    /// </summary>
    /// <param name="range_Id">The ID of the room range</param>
    /// <param name="ra_Id">The ID of the RA to assign</param>
    /// <returns>The created RA_Assigned_Ranges object</returns>
    [HttpPost("assign-ra")]
    public async Task<ActionResult<RA_Assigned_Ranges>> AssignRaToRoomRange([FromBody] RA_AssignmentViewModel model)
    {
        try
        {
            var assignedRange = await housingService.AssignRaToRoomRangeAsync(model.Range_ID, model.Ra_ID);
            return Created("RA assigned to room range successfully.", assignedRange);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the RA assigned to a resident based on their room number and hall ID.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <param name="roomNumber">The resident's room number.</param>
    /// <returns>Returns the RA's ID if found, otherwise null.</returns>
    [HttpGet("GetResidentRA")]
    public async Task<ActionResult<string>> GetResidentRA([FromQuery] string hallId, [FromQuery] string roomNumber)
    {
        try
        {
            var raId = await housingService.GetResidentRAAsync(hallId, roomNumber);
            return Ok(raId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);  // Return 404 if no RA is found for the room in the specified hall
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }



}
