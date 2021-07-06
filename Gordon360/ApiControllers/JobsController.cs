using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService _jobsService;
        private readonly IErrorLogService _errorLogService;

        public JobsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _jobsService = new JobsService(_unitOfWork);
            _errorLogService = new ErrorLogService(_unitOfWork);
        }

        private int GetCurrentUserID()
        {
            int userID = -1;
            var authenticatedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return userID;
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="details"></param>
        /// <returns>The user's active jobs</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<ActiveJobViewModel>> GetJobs(DateTime shiftStart, DateTime shiftEnd) {
            IEnumerable<ActiveJobViewModel> result;
            //try {
                result = _jobsService.getActiveJobs(shiftStart, shiftEnd, GetCurrentUserID());
            //}
            //catch(Exception e) {
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="details"></param>
        /// <returns>The user's active jobs</returns>
        [HttpPost]
        [Route("getJobs")]
        public ActionResult<IEnumerable<ActiveJobViewModel>> DEPRECATED_getJobsForUser([FromBody] ActiveJobSelectionParametersModel details)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = GetCurrentUserID();
            //try
            //{
                result = _jobsService.getActiveJobs(details.SHIFT_START_DATETIME, details.SHIFT_END_DATETIME, userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The user's saved shifts</returns>
        [HttpGet]
        [Route("getSavedShifts/")]
        public ActionResult<StudentTimesheetsViewModel> getSavedShiftsForUser()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StudentTimesheetsViewModel> result = null;

            //try
            //{
                result = _jobsService.getSavedShiftsForUser(userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
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
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> saveShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            //try
            //{

                if (shiftDetails.SHIFT_START_DATETIME == null || shiftDetails.SHIFT_END_DATETIME == null || shiftDetails.SHIFT_START_DATETIME == shiftDetails.SHIFT_END_DATETIME)
                {
                    _errorLogService.Log($"Invalid timesheets shift saved. Student ID: {shiftDetails.ID}, job ID: {shiftDetails.EML}, shiftStart: {shiftDetails.SHIFT_START_DATETIME}, shift end time: {shiftDetails.SHIFT_END_DATETIME}, hours worked: {shiftDetails.HOURS_WORKED} at time {DateTime.Now}");
                    throw new Exception("Invalid shift times. shiftStart and shiftEnd must be non-null and not the same.");
                };

                overlapCheckResult = _jobsService.checkForOverlappingShift(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
                }
                result = _jobsService.saveShiftForUser(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.SHIFT_NOTES, authenticatedUserUsername);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Edit a shift
        /// <param name="shiftDetails">The details that will be changed</param>
        /// </summary>
        [HttpPut]
        [Route("editShift/")]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> editShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            //try
            //{
            overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.ID);
                if (overlapCheckResult.Count() > 0)
                {
                throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
            }
                result = _jobsService.editShift(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, authenticatedUserUsername);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <returns>The result of deleting the shift</returns>
        [HttpDelete]
        [Route("deleteShift/{rowID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> deleteShiftForUser(int rowID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            //try
            //{
                result = _jobsService.deleteShiftForUser(rowID, userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Submit shifts
        /// </summary>
        /// <returns>The result of submitting the shifts</returns>
        [HttpPost]
        [Route("submitShifts")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> submitShiftsForUser([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            //try
            //{
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForUser(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisorName/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<SupervisorViewModel>> getSupervisorName(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            //try
            //{
                result = _jobsService.getsupervisorNameForJob(supervisorID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
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
        public ActionResult<ClockInViewModel> ClockIn([FromBody] bool state)
        {

            if (!ModelState.IsValid || state == null)
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

            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _jobsService.ClockIn(state, authenticatedUserIdString);

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
        public ActionResult<IEnumerable<ClockInViewModel>> ClockOut()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _jobsService.ClockOut(authenticatedUserIdString);

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
        public ActionResult<ClockInViewModel> DeleteClockIn()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _jobsService.DeleteClockIn(authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        ///  gets the response as to whether the user can use staff timesheets
        ///  returns true if the staff member has at least one qualifying active job as hourly staff
        /// </summary>
        /// <returns>Boolean</returns>
        [HttpGet]
        [Route("canUsePage")]
        public ActionResult<IEnumerable<StaffCheckViewModel>> CanUsePage()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _jobsService.CanUsePage(authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        
        //staff routes


        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="details"> deatils of the current Staff</param>
        /// <returns>The Staff's active jobs</returns>
        [HttpPost]
        [Route("jobsStaff")]
        public ActionResult<IEnumerable<ActiveJobViewModel>> getJobsForStaff([FromBody] ActiveJobSelectionParametersModel details)
        {
            IEnumerable<ActiveJobViewModel> result;
            int userID = GetCurrentUserID();
            //try
            //{
                result = _jobsService.getActiveJobsStaff(details.SHIFT_START_DATETIME, details.SHIFT_END_DATETIME, userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The staff's saved shifts</returns>
        [HttpGet]
        [Route("savedShiftsForStaff")]
        public ActionResult<IEnumerable<StaffTimesheetsViewModel>> getSavedShiftsForStaff()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StaffTimesheetsViewModel> result = null;

            //try
            //{
                result = _jobsService.getSavedShiftsForStaff(userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftDetails">The details that will be changed</param>
        /// <returns>The result of saving a shift for a staff</returns>
        [HttpPost]
        [Route("saveShiftStaff")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> saveShiftForStaff([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StaffTimesheetsViewModel> result;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult;

            int userID = GetCurrentUserID();
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //try
            //{
                overlapCheckResult = _jobsService.checkForOverlappingShiftStaff(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
                }
                result = _jobsService.saveShiftForStaff(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.HOURS_TYPE, shiftDetails.SHIFT_NOTES, authenticatedUserIdString);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Edit a shift for staff
        /// <param name="shiftDetails">The details that will be changed</param>
        /// </summary>
        [HttpPut]
        [Route("editShiftStaff")]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> editShiftForStaff([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            //try
            //{
            overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.ID);
                if (overlapCheckResult.Count() > 0)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Error: shift overlap detected" };
                }
                result = _jobsService.editShiftStaff(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, authenticatedUserUsername);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Delete a user's active job
        /// </summary>
        /// <returns>The result of deleting the shift for a Staff</returns>
        [HttpDelete]
        [Route("deleteShiftStaff/{rowID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> deleteShiftForStaff(int rowID)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            //try
            //{
                result = _jobsService.deleteShiftForStaff(rowID, userID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Submit shift for staff
        /// </summary>
        /// <returns>The result of submitting the shifts for staff</returns>
        [HttpPost]
        [Route("submitShiftsStaff")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<StudentTimesheetsViewModel>> submitShiftsForStaff([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            //try
            //{
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForStaff(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number for Staff
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisorNameStaff/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public ActionResult<IEnumerable<SupervisorViewModel>> getSupervisorNameStaff(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            //try
            //{
                result = _jobsService.getStaffSupervisorNameForJob(supervisorID);
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }

        /// <summary>
        /// Gets the hour types for Staff
        /// </summary>
        /// <returns>The hour types for staff</returns>
        [HttpGet]
        [Route("hourTypes")]
        public ActionResult<IEnumerable<HourTypesViewModel>> getHourTypes()
        {
            IEnumerable<HourTypesViewModel> result = null;

            //try
            //{
                result = _jobsService.GetHourTypes();
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //    return InternalServerError();
            //}
            return Ok(result);
        }
    }
}
