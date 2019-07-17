


using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/schedulecontrol")]
    [CustomExceptionFilter]
    [Authorize]
    public class ScheduleControlController : ApiController
    {
        private IUnitOfWork _unitOfWork;

        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IScheduleControlService _scheduleControlService;


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
        /// Get schedule information of local user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;

            object scheduleControlResult = null;
            
            scheduleControlResult = _unitOfWork.ScheduleControlRepository.GetById(id);

            if (scheduleControlResult == null)
            {
                return NotFound();
            }

            return Ok(scheduleControlResult);

        }



        /// <summary>
        /// Get schedule information of specific user
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            object scheduleControlResult = null;

            var id = _accountService.GetAccountByUsername(username).GordonID;
            scheduleControlResult = _unitOfWork.ScheduleControlRepository.GetById(id);
            
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
        [Route("update/privacy/{value}")]
        public IHttpActionResult UpdateSchedulePrivacy(string value)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            _scheduleControlService.UpdateSchedulePrivacy(id, value);

            return Ok();

        }

        /// <summary>
        /// Update schedule description
        /// </summary>
        /// <param name="value">New description</param>
        /// <returns></returns>
        [HttpPut]
        [Route("update/description/{value}")]
        public IHttpActionResult UpdateDescription(string value)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            _scheduleControlService.UpdateDescription(id, value);

            return Ok();

        }

        /// <summary>
        /// Update timestamp of last modified schedule
        /// </summary>
        /// <param name="value">Datetime in string</param>
        /// <returns></returns>
        [HttpPut]
        [Route("update/timestamp/{value}")]
        public IHttpActionResult UpdateModifiedTimeStamp(string value)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            _scheduleControlService.UpdateModifiedTimeStamp(id, Convert.ToDateTime(value));

            return Ok();

        }

    }

}
