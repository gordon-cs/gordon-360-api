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
using System.Collections.Generic;

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
    /// <param name="assignmentRange">The ViewModel that contains the hall ID and room range</param>
    /// <returns>The created Hall_Assignment_Range object</returns>
    [HttpPost("roomrange")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING_ROOM_RANGE)]
    public async Task<ActionResult<Hall_Assignment_Ranges>> CreateRoomRange([FromBody] HallAssignmentRangeViewModel assignmentRange)
    {
        try
        {
            var newRange = await housingService.CreateRoomRangeAsync(assignmentRange);
            return Created("Room range created successfully.", newRange);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves all room ranges.
    /// </summary>
    /// <returns>A list of room ranges.</returns>
    [HttpGet("roomranges")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.HOUSING_ROOM_RANGE)]
    public async Task<ActionResult<List<HallAssignmentRangeViewModel>>> GetAllRoomRanges()
    {
        var roomRanges = await housingService.GetAllRoomRangesAsync();
        return Ok(roomRanges);
    }

    /// <summary>
    /// Deletes a Room Range
    /// </summary>
    /// <param name="rangeId">The ID of the room range to delete</param>
    /// <returns> Returns if completed</returns>
    [HttpDelete("roomranges/{rangeId}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.HOUSING_ROOM_RANGE)]
    public async Task<IActionResult> DeleteRoomRange([FromRoute] int rangeId)
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


    /// <summary>
    /// Assigns an RA to a room range if no RA is currently assigned
    /// </summary>
    /// <param name="assignment">The model containging the RA and range ids</param>
    /// <returns>The created RA_Assigned_Ranges object</returns>
    [HttpPost("roomrangeassignments")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING_RA_ASSIGNMENT)]
    public async Task<ActionResult<Hall_Assignment_Ranges>> AssignRaToRoomRange([FromBody] RA_AssignmentViewModel assignment)
    {
        try
        {
            var assignedRange = await housingService.AssignRaToRoomRangeAsync(assignment.Range_ID, assignment.Ra_ID);
            return Created("RA assigned to room range successfully.", assignedRange);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves the list of all ra roomrange assignments.
    /// </summary>
    /// <returns>Returns a list of all assignments</returns>
    [HttpGet]
    [Route("roomrangeassignments")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.HOUSING_RA_ASSIGNMENT)]
    public async Task<IActionResult> GetRangeAssignments()
    {
        var RangeAssignments = await housingService.GetRangeAssignmentsAsync();
        if (RangeAssignments == null)
        {
            return NotFound("No Assigned Ranges");
        }
        return Ok(RangeAssignments);
    }

    /// <summary>
    /// Deletes an RA range assignment
    /// </summary>
    /// <param name="rangeId">The Room range of the assignment to delete</param>
    /// <returns> Returns if completed</returns>
    [HttpDelete("roomranges/assignment/{rangeId}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.HOUSING_RA_ASSIGNMENT)]
    public async Task<IActionResult> DeleteAssignment([FromRoute] int rangeId)
    {
        var success = await housingService.DeleteAssignmentAsync(rangeId);
        if (success)
        {
            return Ok(new { message = "Assigment deleted successfully." });
        }
        else
        {
            return BadRequest(new { message = "Failed to delete the Assignment." });
        }
    }

    /// <summary>
    /// Retrieve the RD of the resident's hall based on their hall ID.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <returns>Returns the RD's details if found, otherwise null.</returns>
    [HttpGet("rds")]
    public async Task<IActionResult> GetResidentRD([FromQuery] string hallId)
    {
        var rdInfo = await housingService.GetResidentRDAsync(hallId);
        return Ok(rdInfo);
    }

    /// <summary>
    /// Retrieves the RA assigned to a resident based on their room number and hall ID.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <param name="roomNumber">The resident's room number.</param>
    /// <returns>Returns the RA's details if found, otherwise null. </returns>
    [HttpGet("resident/ra")]
    public async Task<IActionResult> GetResidentRA([FromQuery] string hallId, [FromQuery] string roomNumber)
    {
        try
        {
            var raInfo = await housingService.GetResidentRAAsync(hallId, roomNumber);
            return Ok(raInfo);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);  // Return 404 if no RA is found for the room in the specified hall
        }
    }

    /// <summary>
    /// Retrieves a list of all RAs.
    /// </summary>
    /// <returns>Returns a list of RA_Students containing information about each RA</returns>
    [HttpGet]
    [Route("ras")]
    public async Task<IActionResult> GetAllRAs()
    {
        var raList = await housingService.GetAllRAsAsync();
        if (raList == null)
        {
            return NotFound("No Resident Advisors found.");
        }
        return Ok(raList);
    }

    /// <summary>
    /// Sets or updates an RA's preferred contact method
    /// </summary>
    /// <param name="raId">The ID of the RA</param>
    /// <param name="preferredContactMethod">The contact method (e.g., "Phone", "Teams")</param>
    /// <returns>True if the contact method was successfully set</returns>
    [HttpPost("ras/{raId}/contact")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING_CONTACT_PREFERENCE)]
    public async Task<IActionResult> SetPreferredContact([FromRoute] string raId, [FromQuery] string preferredContactMethod)
    {
        if (string.IsNullOrWhiteSpace(raId) || string.IsNullOrWhiteSpace(preferredContactMethod))
        {
            return BadRequest("RA ID and contact method are required.");
        }

        var result = await housingService.SetPreferredContactMethodAsync(raId, preferredContactMethod);
        if (result)
        {
            return Ok("Preferred contact method set successfully.");
        }
        else
        {
            return StatusCode(500, "An error occurred while setting the preferred contact method.");
        }
    }

    /// <summary>
    /// Retrieves the preferred contact information for an RA based on their contact preference.
    /// If the RA has a contact preference set, it will return either their phone number or a Microsoft Teams link 
    /// with their email embedded. If no preference exists, the method defaults to returning the RA's phone number.
    /// </summary>
    /// <param name="raId">The ID of the RA whose contact information is being requested.</param>
    /// <returns>An object containing the RAiD, preferred method, and the actual contact.</returns>
    [HttpGet("ra/{raId}/contact")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING_CONTACT_PREFERENCE)]
    public async Task<ActionResult<RA_ContactPreference>> GetRAContact([FromRoute] string raId)
    {
        var contactInfo = await housingService.GetPreferredContactAsync(raId);

        if (contactInfo.Contact == null)
        {
            return NotFound("RA contact information not found.");
        }

        return Ok(contactInfo);
    }


    /// <summary>
    /// Creates a new status event for an RA schedule
    /// </summary>
    /// <param name="Status_Sched">The ViewModel that contains the Schedule ID and RA ID</param>
    /// <param name="raId">the id of the RA setting a status</param>
    /// <returns>The created RA_Status_Schedule object</returns>
    [HttpPost("ras/{raId}/status")]
    public async Task<ActionResult<RA_Status_Schedule>> CreateStatus( [FromBody] RA_Status_ScheduleViewModel Status_Sched, [FromRoute] string raId)
    {
        try
        {
            var newStatus = await housingService.CreateStatusAsync(Status_Sched,raId);
            return Created("Status event created successfully.", newStatus);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Checks an RA in
    /// </summary>
    /// <param name="raId">Id of the ra checking in</param>
    ///<param name="HallIDs">The Hall(s) the RA is checking into</param>
    /// <returns>true if RA checked in successfully</returns>
    [HttpPost("ras/{raId}/checkin")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RA_CHECKIN)]
    public async Task<ActionResult<bool>> RA_Checkin([FromRoute] string raId,[FromBody] string[] HallIDs)
    {
        try
        {
            var checkedIn = await housingService.RA_CheckinAsync(HallIDs, raId);
            if (checkedIn)
            {
                return Created("RA checked in successfully.", checkedIn);
            }
            return BadRequest("Failed to check in RA.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets the on-call RA for a specified hall.
    /// </summary>
    /// <param name="hallid">The ID of the hall</param>
    /// <returns> the on-call RA's details, or a 404 if no RA is on call</returns>
    [HttpGet("halls/{hallid}/on-call")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING_ON_CALL_RA)]
    public async Task<ActionResult<string>> GetOnCallRA([FromRoute] string hallid)
    {
        var raId = await housingService.GetOnCallRAAsync(hallid);

        if (raId == null)
        {
            return NotFound($"No RA is currently on call for selected Hall");
        }

        return Ok(raId);
    }

    /// <summary>
    /// Gets the on-call RAs for all halls.
    /// </summary>
    /// <returns>The RAs on call with their details</returns>
    [HttpGet("halls/on-calls")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.HOUSING_ON_CALL_RA)]
    public async Task<ActionResult<List<RA_On_Call_GetViewModel>>> GetOnCallRAAllHalls()
    {
        var onCallRAs = await housingService.GetOnCallRAAllHallsAsync();

        if (onCallRAs == null || !onCallRAs.Any())
        {
            return NotFound("No RA is currently on call for any hall.");
        }

        return Ok(onCallRAs);
    }

    /// <summary>
    /// Checks if an RA is currently on call.
    /// </summary>
    /// <param name="raId">The ID of the RA</param>
    /// <returns>True if the RA is on call, false otherwise</returns>
    [HttpGet("ras/{raId}/is-on-call")]
    public async Task<IActionResult> IsRAOnCall([FromRoute] string raId)
    {
        var isOnCall = await housingService.IsRAOnCallAsync(raId);

        return Ok(new { IsOnCall = isOnCall });
    }

    /// <summary>
    /// Checks if a student is residential
    /// </summary>
    /// <param name="idNum">The ID of the student</param>
    /// <returns>True if the student is a resident</returns>
    [HttpGet]
    [Route("students/{idNum}/is-residential")]
    public async Task<IActionResult> IsStudentResidential([FromRoute] int idNum)
    {
        try
        {
            var isResidential = await housingService.IsStudentResidentialAsync(idNum);
            return Ok(new { IsResidential = isResidential });
        }
        catch (ResourceNotFoundException)
        {
            return NotFound($"Student with ID {idNum} not found.");
        }
    }




}
