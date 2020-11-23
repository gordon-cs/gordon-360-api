using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;
using Gordon360.Services.ComplexQueries;
using Gordon360.Services;
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/jobs")]
    public class JobsController : ApiController
    {
        private IJobsService _jobsService;
        private IAccountService _accountService;

        public JobsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _jobsService = new JobsService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        private int GetCurrentUserID()
        {
            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            string id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            return userID;
        }

        /// <summary>
        /// Gets jobs for the currently authenticated user that are active in the specified shift timespan
        /// </summary>
        /// <param name="shiftStart"> DateTime when shift started </param>
        /// <param name="shiftEnd"> Datetime when shift ended </param>
        /// <returns>The user's active jobs</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetJobs(DateTime shiftStart, DateTime shiftEnd)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = GetCurrentUserID();
            try
            {
                result = _jobsService.getActiveJobs(shiftStart, shiftEnd, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets saved shifts for the currently authenticated user
        /// </summary>
        /// <returns>The user's saved shifts</returns>
        [HttpGet]
        [Route("shifts")]
        public HttpResponseMessage GetShifts()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StudentTimesheetsViewModel> result = null;

            try
            {
                result = _jobsService.getSavedShiftsForUser(userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Saves a shift
        /// </summary>
        /// <param name="shiftDetails"></param>
        /// <returns>The result of saving a shift</returns>
        [HttpPost]
        [Route("shifts")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public HttpResponseMessage SaveShift([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            try
            {
                overlapCheckResult = _jobsService.checkForOverlappingShift(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.saveShiftForUser(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.SHIFT_NOTES, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Edit a shift
        /// <param name="shiftDetails">The details that will be changed</param>
        /// <param name="shiftID"> The id of the shift to change</param>
        /// </summary>
        [HttpPut]
        [Route("shifts/{shiftID}")]
        public HttpResponseMessage EditShift([FromBody] ShiftViewModel shiftDetails, [FromUri] int shiftID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            try
            {
                overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftID);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.editShift(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a shift
        /// </summary>
        /// <param name="shiftID">ID of shift to delete</param>
        /// <returns>The result of deleting the shift</returns>
        [HttpDelete]
        [Route("shifts/{shiftID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public IHttpActionResult DeleteShift(int shiftID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                result = _jobsService.deleteShiftForUser(shiftID, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Submit shifts
        /// </summary>
        /// <returns>The result of submitting the shifts</returns>
        [HttpPost]
        [Route("shifts/submit")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult SubmitShifts([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForUser(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number
        /// </summary>
        /// <param name="supervisorID">ID of supervisor to get.</param>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisor/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult GetSupervisorName(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            try
            {
                result = _jobsService.getsupervisorNameForJob(supervisorID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        ///  gets the the clock in status from the back end
        /// </summary>
        /// <returns>ClockInViewModel</returns>
        [HttpGet]
        [Route("clockins")]
        public IHttpActionResult GetClockIns()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.ClockOut(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  sends the current clock in status to the back end
        /// </summary>
        /// <param name="state">detail to be saved in the back end, true if user just clocked in</param>
        /// <returns>returns confirmation that the answer was recorded </returns>
        [HttpPost]
        [Route("clockins")]
        public IHttpActionResult PostClockIn([FromBody] bool state)
        {

            if (!ModelState.IsValid)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.ClockIn(state, id);

            if (result == null)
            {
                return NotFound();
            }

            return Created("Recorded answer :", result);
        }

        /// <summary>
        /// deletes the last clocked in status of a user
        /// </summary>
        /// <returns>returns confirmation that clock in status was deleted</returns>
        [HttpDelete]
        [Route("clockins")]
        public IHttpActionResult DeleteClockIn()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.DeleteClockIn(id);

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
        /// <param name="shiftStart">Start of shift</param>
        /// <param name="shiftEnd">End of shift</param>
        /// <returns>The Staff's active jobs</returns>
        [HttpPost]
        [Route("staff")]
        public IHttpActionResult GetStaffJobs(DateTime shiftStart, DateTime shiftEnd)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = GetCurrentUserID();
            try
            {
                result = _jobsService.getActiveJobsStaff(shiftStart, shiftEnd, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The staff's saved shifts</returns>
        [HttpGet]
        [Route("staff/shifts")]
        public HttpResponseMessage GetStaffShifts()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StaffTimesheetsViewModel> result = null;

            try
            {
                result = _jobsService.getSavedShiftsForStaff(userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Posts a shift
        /// </summary>
        /// <param name="shiftDetails">The details that will be changed</param>
        /// <returns>The result of saving a shift for a staff</returns>
        [HttpPost]
        [Route("staff/shifts")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public HttpResponseMessage PostStaffShift([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            try
            {
                overlapCheckResult = _jobsService.checkForOverlappingShiftStaff(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.saveShiftForStaff(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.HOURS_TYPE, shiftDetails.SHIFT_NOTES, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Edit a shift for staff
        /// <param name="shiftDetails">The details that will be changed</param>
        /// <param name="shiftID">Id of shift to edit</param>
        /// </summary>
        [HttpPut]
        [Route("staff/shifts/{shiftID}")]
        public HttpResponseMessage EditStaffShift([FromBody] ShiftViewModel shiftDetails, [FromUri] int shiftID)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            try
            {
                overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftID);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.editShiftStaff(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a user's active job
        /// </summary>
        /// <returns>The result of deleting the shift for a Staff</returns>
        [HttpDelete]
        [Route("staff/shifts/{shiftID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public IHttpActionResult DeleteStaffShift(int shiftID)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                result = _jobsService.deleteShiftForStaff(shiftID, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Submit shift for staff
        /// </summary>
        /// <param name="shifts">List of shifts to submit</param>
        /// <returns>The result of submitting the shifts for staff</returns>
        [HttpPost]
        [Route("staff/shifts/submit")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult SubmitStaffShifts([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForStaff(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number for Staff
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("staff/supervisor/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult GetStaffSupervisor(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            try
            {
                result = _jobsService.getStaffSupervisorNameForJob(supervisorID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the hour types for Staff
        /// </summary>
        /// <returns>The hour types for staff</returns>
        [HttpGet]
        [Route("staff/types")]
        public IHttpActionResult GetHourTypes()
        {
            IEnumerable<HourTypesViewModel> result = null;

            try
            {
                result = _jobsService.GetHourTypes();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="details"></param>
        /// <returns>The user's active jobs</returns>
        [HttpPost]
        [Route("getJobs")]
        public IHttpActionResult DEPRECATED_getJobsForUser([FromBody] ActiveJobSelectionParametersModel details)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = GetCurrentUserID();
            try
            {
                result = _jobsService.getActiveJobs(details.SHIFT_START_DATETIME.ToLocalTime(), details.SHIFT_END_DATETIME.ToLocalTime(), userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The user's saved shifts</returns>
        [HttpGet]
        [Route("getSavedShifts/")]
        public HttpResponseMessage DEPRECATED_getSavedShiftsForUser()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StudentTimesheetsViewModel> result = null;

            try
            {
                result = _jobsService.getSavedShiftsForUser(userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftDetails"></param>
        /// <returns>The result of saving a shift</returns>
        [HttpPost]
        [Route("saveShift")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public HttpResponseMessage DEPRECATED_saveShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            try
            {
                overlapCheckResult = _jobsService.checkForOverlappingShift(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.saveShiftForUser(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.SHIFT_NOTES, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Edit a shift
        /// <param name="shiftDetails">The details that will be changed</param>
        /// </summary>
        [HttpPut]
        [Route("editShift/")]
        public HttpResponseMessage DEPRECATED_editShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            try
            {
                overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.ID);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.editShift(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <returns>The result of deleting the shift</returns>
        [HttpDelete]
        [Route("deleteShift/{rowID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_deleteShiftForUser(int rowID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                result = _jobsService.deleteShiftForUser(rowID, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Submit shifts
        /// </summary>
        /// <returns>The result of submitting the shifts</returns>
        [HttpPost]
        [Route("submitShifts")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_submitShiftsForUser([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForUser(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisorName/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_getSupervisorName(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            try
            {
                result = _jobsService.getsupervisorNameForJob(supervisorID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
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
        public IHttpActionResult DEPRECATED_ClockIn([FromBody] bool state)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.ClockIn(state, id);

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
        public IHttpActionResult DEPRECATED_ClockOut()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.ClockOut(id);

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
        public IHttpActionResult DEPRECATED_DeleteClockIn()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.DeleteClockIn(id);

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
        public IHttpActionResult CanUsePage()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _jobsService.CanUsePage(id);

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
        public IHttpActionResult DEPRECATED_getJobsForStaff([FromBody] ActiveJobSelectionParametersModel details)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = GetCurrentUserID();
            try
            {
                result = _jobsService.getActiveJobsStaff(details.SHIFT_START_DATETIME, details.SHIFT_END_DATETIME, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The staff's saved shifts</returns>
        [HttpGet]
        [Route("savedShiftsForStaff")]
        public HttpResponseMessage DEPRECATED_getSavedShiftsForStaff()
        {
            int userID = GetCurrentUserID();

            IEnumerable<StaffTimesheetsViewModel> result = null;

            try
            {
                result = _jobsService.getSavedShiftsForStaff(userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftDetails">The details that will be changed</param>
        /// <returns>The result of saving a shift for a staff</returns>
        [HttpPost]
        [Route("saveShiftStaff")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public HttpResponseMessage DEPRECATED_saveShiftForStaff([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = GetCurrentUserID();
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            try
            {
                overlapCheckResult = _jobsService.checkForOverlappingShiftStaff(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.saveShiftForStaff(userID, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.HOURS_TYPE, shiftDetails.SHIFT_NOTES, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Edit a shift for staff
        /// <param name="shiftDetails">The details that will be changed</param>
        /// </summary>
        [HttpPut]
        [Route("editShiftStaff")]
        public HttpResponseMessage DEPRECATED_editShiftForStaff([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            IEnumerable<OverlappingShiftIdViewModel> overlapCheckResult = null;

            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            try
            {
                overlapCheckResult = _jobsService.editShiftOverlapCheck(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.ID);
                if (overlapCheckResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Error: shift overlap detected");
                }
                result = _jobsService.editShiftStaff(shiftDetails.ID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, username);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a user's active job
        /// </summary>
        /// <returns>The result of deleting the shift for a Staff</returns>
        [HttpDelete]
        [Route("deleteShiftStaff/{rowID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_deleteShiftForStaff(int rowID)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                result = _jobsService.deleteShiftForStaff(rowID, userID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Submit shift for staff
        /// </summary>
        /// <returns>The result of submitting the shifts for staff</returns>
        [HttpPost]
        [Route("submitShiftsStaff")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_submitShiftsForStaff([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            int userID = GetCurrentUserID();

            try
            {
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForStaff(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the name of a supervisor based on their ID number for Staff
        /// </summary>
        /// <returns>The name of the supervisor</returns>
        [HttpGet]
        [Route("supervisorNameStaff/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult DEPRECATED_getSupervisorNameStaff(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            try
            {
                result = _jobsService.getStaffSupervisorNameForJob(supervisorID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the hour types for Staff
        /// </summary>
        /// <returns>The hour types for staff</returns>
        [HttpGet]
        [Route("hourTypes")]
        public IHttpActionResult DEPRECATED_getHourTypes()
        {
            IEnumerable<HourTypesViewModel> result = null;

            try
            {
                result = _jobsService.GetHourTypes();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

    }

}
