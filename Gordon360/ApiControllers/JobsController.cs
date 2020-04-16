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

        /// <summary>
        /// This is just an end-to-end proof of concept.
        /// It will return some dummy text.
        /// <returns> A dummy string </returns>
        /// </summary>
        [HttpGet]
        [Route("hello-world")]
        public IHttpActionResult GetTestData()
        {
            return Ok("Hello World! We connected to the back-end.");
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="details"></param>
        /// <returns>The user's active jobs</returns>
        [HttpPost]
        [Route("getJobs")]
        public IHttpActionResult getJobsForUser([FromBody] ActiveJobSelectionParametersModel details)
        {
            IEnumerable<ActiveJobViewModel> result = null;
            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);
            try
            {
                result = _jobsService.getActiveJobs(details.SHIFT_START_DATETIME, details.SHIFT_END_DATETIME, userID);
            }
            catch (Exception e)
            {
                //
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a user's saved shifts
        /// </summary>
        /// <returns>The user's active jobs</returns>
        [HttpGet]
        [Route("getSavedShifts/")]
        public HttpResponseMessage getSavedShiftsForUser()
        {
            int userID = -1;

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);
            
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
        /// <returns>The user's active jobs</returns>
        [HttpPost]
        [Route("saveShift")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public HttpResponseMessage saveShiftForUser([FromBody] ShiftViewModel shiftDetails)
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
        public HttpResponseMessage editShiftForUser([FromBody] ShiftViewModel shiftDetails)
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
                overlapCheckResult = _jobsService.checkForOverlappingShift(userID, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME);
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
        public IHttpActionResult deleteShiftForUser(int rowID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            int userID = -1;

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

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
        /// <returns>The result of deleting the shift</returns>
        [HttpPost]
        [Route("submitShifts")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult submitShiftsForUser([FromBody] IEnumerable<ShiftToSubmitViewModel> shifts)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            try
            {
                foreach (ShiftToSubmitViewModel shift in shifts)
                {
                    result = _jobsService.submitShiftForUser(shift.ID_NUM, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
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
        /// Submit shifts
        /// </summary>
        /// <returns>The result of deleting the shift</returns>
        [HttpGet]
        [Route("supervisorName/{supervisorID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.SHIFT)]
        public IHttpActionResult submitShiftsForUser(int supervisorID)
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
    }
}
