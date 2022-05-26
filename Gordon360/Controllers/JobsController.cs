using Gordon360.AuthorizationFilters;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.StudentTimesheets.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : GordonControllerBase
    {
        private readonly IJobsService _jobsService;
        private readonly IAccountService _accountService;
        private readonly IErrorLogService _errorLogService;

        public JobsController(StudentTimesheetsContext context, CCTContext cctContext)
        {
            _jobsService = new JobsService(context, cctContext);
            _errorLogService = new ErrorLogService(cctContext);
            _accountService = new AccountService(cctContext);
        }

        private int GetCurrentUserID()
        {
            var username = AuthUtils.GetUsername(User);
            var account = _accountService.GetAccountByUsername(username);
            return int.Parse(account.GordonID);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftStart">The datetime that the shift started</param>
        /// <param name="shiftEnd">The datetime that the shift ended</param>
        /// <returns>The user's active jobs</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<ActiveJobViewModel>>> GetJobsAsync(DateTime shiftStart, DateTime shiftEnd)
        {
            var result = await _jobsService.GetActiveJobsAsync(shiftStart, shiftEnd, GetCurrentUserID());
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The user's saved shifts</returns>
        [HttpGet]
        [Route("getSavedShifts/")]
        public ActionResult<StudentTimesheetsViewModel> GetSavedShiftsForUser()
        {
            int userID = GetCurrentUserID();
            var result = _jobsService.getSavedShiftsForUser(userID);
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftDetails"></param>
        /// <returns>The result of saving a shift</returns>
        [HttpPost]
        [Route("saveShift")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public async Task<ActionResult> SaveShiftForUserAsync([FromBody] ShiftViewModel shiftDetails)
        {
            int userID = GetCurrentUserID();
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            if (shiftDetails.SHIFT_START_DATETIME == null || shiftDetails.SHIFT_END_DATETIME == null || shiftDetails.SHIFT_START_DATETIME == shiftDetails.SHIFT_END_DATETIME)
            {
                _errorLogService.Log($"Invalid timesheets shift saved. Student ID: {shiftDetails.ID}, job ID: {shiftDetails.EML}, shiftStart: {shiftDetails.SHIFT_START_DATETIME}, shift end time: {shiftDetails.SHIFT_END_DATETIME}, hours worked: {shiftDetails.HOURS_WORKED} at time {DateTime.Now}");
                throw new Exception("Invalid shift times. shiftStart and shiftEnd must be non-null and not the same.");
            };

            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = await _jobsService.CheckForOverlappingShiftAsync(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
            if (overlapCheckResult.Any())
            {
                throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
            }
            await _jobsService.SaveShiftForUserAsync(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.SHIFT_NOTES, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Edit a shift
        /// <param name="shiftDetails">The details that will be changed</param>
        /// </summary>
        [HttpPut]
        [Route("editShift/")]
        public async Task<ActionResult<StudentTimesheetsViewModel>> EditShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {

            int userID = GetCurrentUserID();
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            var overlapCheckResult = await _jobsService.EditShiftOverlapCheckAsync(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.ID);
            if (overlapCheckResult.Any())
            {
                throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
            }
            var result = _jobsService.EditShift(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, authenticatedUserUsername);
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <returns>The result of deleting the shift</returns>
        [HttpDelete]
        [Route("deleteShift/{rowID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public ActionResult DeleteShiftForUser(int rowID)
        {
            int userID = GetCurrentUserID();

            //try
            //{
            _jobsService.DeleteShiftForUser(rowID, userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok();
        }

        /// <summary>
        /// Submit shifts
        /// </summary>
        /// <returns>The result of submitting the shifts</returns>
        [HttpPost]
        [Route("submitShifts")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public async Task<ActionResult> SubmitShiftsForUser([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            int userID = GetCurrentUserID();

            //try
            //{
            foreach (ShiftToSubmitViewModel shift in shifts)
            {
                await _jobsService.SubmitShiftForUserAsync(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
            }
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok();
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisorName/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public async Task<ActionResult<IEnumerable<SupervisorViewModel>>> GetSupervisorNameAsync(int supervisorID)
        {
            var result = await _jobsService.GetsupervisorNameForJobAsync(supervisorID);
            return Ok(result);
        }

        /// <summary>
        ///  sends the current clock in status to the back end
        ///  true if user is clocked in and false if clocked out
        /// </summary>
        /// <param name="state">detail to be saved in the back end, true if user just clocked in</param>
        /// <returns>returns confirmation that the answer was recorded </returns>
        [HttpPost]
        [Route("clockIn")]
        public async Task<ActionResult<ClockInViewModel>> ClockInAsync([FromBody] bool state)
        {
            var userID = GetCurrentUserID().ToString();
            var result = await _jobsService.ClockInAsync(state, userID);

            if (result == null)
            {
                return NotFound();
            }

            return Created("Recorded answer :", result);
        }

        /// <summary>
        ///  gets the the clock in status from the back end
        ///  true if user is clocked in and false if clocked out
        /// </summary>
        /// <returns>ClockInViewModel</returns>
        [HttpGet]
        [Route("clockOut")]
        public async Task<ActionResult<ClockInViewModel>> ClockOutAsync()
        {
            var userID = GetCurrentUserID().ToString();
            var result = await _jobsService.ClockOutAsync(userID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// deletes the last clocked in status of a user
        /// </summary>
        /// <returns>returns confirmation that clock in status was deleted</returns>
        [HttpPut]
        [Route("deleteClockIn")]
        public async Task<ActionResult<ClockInViewModel>> DeleteClockInAsync()
        {
            var userID = GetCurrentUserID().ToString();
            var result = await _jobsService.DeleteClockInAsync(userID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
