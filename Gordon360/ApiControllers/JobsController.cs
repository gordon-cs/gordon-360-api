using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
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

        public JobsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _jobsService = new JobsService(_unitOfWork);
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
            System.Diagnostics.Debug.WriteLine("getting jobs");
            IEnumerable<ActiveJobViewModel> result = null;
            try
            {
                result = _jobsService.getActiveJobs(details.SHIFT_START_DATETIME, details.SHIFT_END_DATETIME, details.ID_NUM);
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
        /// <param name="userID">The user's Gordon ID</param>
        /// <returns>The user's active jobs</returns>
        [HttpGet]
        [Route("getSavedShifts/{userID}")]
        public IHttpActionResult getSavedShiftsForUser(string userID)
        {
            string query = "SELECT * from student_timesheets where ID_NUM = " + userID + ";";
            IEnumerable<StudentTimesheetsViewModel> queryResult = null;
            try
            {
                queryResult = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery(query);
            }
            catch (Exception e)
            {
                //
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return Ok(queryResult);
        }

        /// <summary>
        /// Get a user's active jobs
        /// </summary>
        /// <param name="shiftDetails"></param>
        /// <returns>The user's active jobs</returns>
        [HttpPost]
        [Route("saveShift")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SHIFT)]
        public IHttpActionResult saveShiftForUser([FromBody] ShiftViewModel shiftDetails)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            try
            {
                result = _jobsService.saveShiftForUser(shiftDetails.ID_NUM, shiftDetails.EML, shiftDetails.SHIFT_START_DATETIME, shiftDetails.SHIFT_END_DATETIME, shiftDetails.HOURS_WORKED, shiftDetails.SHIFT_NOTES, shiftDetails.LAST_CHANGED_BY);
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
        /// <returns>The result of deleting the shift</returns>
        [HttpDelete]
        [Route("deleteShift/{rowID}/{userID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SHIFT)]
        public IHttpActionResult deleteShiftForUser(int rowID, int userID)
        {
            System.Diagnostics.Debug.WriteLine("deleting shift");
            IEnumerable<StudentTimesheetsViewModel> result = null;

            System.Diagnostics.Debug.WriteLine("Row id: " + rowID);
            System.Diagnostics.Debug.WriteLine("Student id: " + userID);

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
            System.Diagnostics.Debug.WriteLine("submitting shifts");
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
