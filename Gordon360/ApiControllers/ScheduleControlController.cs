using Gordon360.Services;
using Gordon360.Repositories;
using System;
using Newtonsoft.Json.Linq;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gordon360.Controllers.Api
{
    [Route("api/schedulecontrol")]
    [CustomExceptionFilter]
    [Authorize]
    public class ScheduleControlController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IScheduleControlService _scheduleControlService;

        public ScheduleControlController()
        {
            _unitOfWork = new UnitOfWork();
            _scheduleControlService = new ScheduleControlService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        public ScheduleControlController(IScheduleControlService scheduleControlService)
        {
            _scheduleControlService = scheduleControlService;
        }

        /// <summary>
        /// Get schedule information of logged in user
        /// Info one gets: privacy, time last updated, description, and Gordon ID
        /// @TODO: Use Service Layer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<JObject> Get()
        {
            var authenticatedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            object scheduleControlResult = _unitOfWork.ScheduleControlRepository.GetById(authenticatedUserId);

            if (scheduleControlResult == null)
            {
                return NotFound();
            }

            JObject jresult = JObject.FromObject(scheduleControlResult);

            jresult.Property("gordon_id").Remove();

            return Ok(jresult);
        }


        /// <summary>
        /// Get schedule information of specific user
        /// Info one gets: privacy, time last updated, description, and Gordon ID
        /// @TODO Use Service Layer
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}")]
        public ActionResult<object> Get(string username)
        {
            var id = _accountService.GetAccountByUsername(username).GordonID;
            object scheduleControlResult = _unitOfWork.ScheduleControlRepository.GetById(id);
            
            if (scheduleControlResult == null)
            {
                return NotFound();
            }

            return Ok(scheduleControlResult);
        }

        /// <summary>
        /// Update privacy of schedule
        /// </summary>
        /// <param name="value">Y or N</param>
        /// <returns></returns>
        [HttpPut]
        [Route("privacy/{value}")]
        public ActionResult UpdateSchedulePrivacy(string value)
        {
            // Verify Input
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

            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _scheduleControlService.UpdateSchedulePrivacy(authenticatedUserIdString, value);

            return Ok();
        }

        /// <summary>
        /// Update schedule description
        /// </summary>
        /// <param name="value">New description</param>
        /// <returns></returns>
        [HttpPut]
        [Route("description/{value}")]
        public ActionResult UpdateDescription(string value)
        {

            DateTime localDate = DateTime.Now;

            // Verify Input
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

            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _scheduleControlService.UpdateDescription(authenticatedUserIdString, value);
            _scheduleControlService.UpdateModifiedTimeStamp(authenticatedUserIdString, localDate);

            return Ok();
        }
    }
}
