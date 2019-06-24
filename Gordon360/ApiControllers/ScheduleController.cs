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
    [RoutePrefix("api/schedule")]
    [CustomExceptionFilter]
    [Authorize]
    public class ScheduleController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IScheduleService _scheduleService;

        public ScheduleController()
        {
            var _unitOfWork = new UnitOfWork();
            _scheduleService = new ScheduleService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;
            var idInt = Int32.Parse(id);

            var result = _scheduleService.Get(idInt);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            //probably needs privacy stuff like ProfilesController and service
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var idInt = Int32.Parse(id);

            var result = _scheduleService.Get(idInt);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        /// <summary>Create a new schedule to be added to database</summary>
        /// <param name="schedule">The schedule item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new schedule to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Post([FromBody] SCHEDULE schedule)
        {
            if (!ModelState.IsValid || schedule == null)
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

            var result = _scheduleService.Add(schedule);

            if (result == null)
            {
                return NotFound();
            }

            return Created("schedule", schedule);
        }

        /// <summary>Delete an existing schedule item</summary>
        /// <param name="id">The identifier for the schedule to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given schedule from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult Delete(int id) //TODO: MAKE THIS USE THE KEY OF THE SCHEDULE TABLE
        {
            var result = _scheduleService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Delete all schedule items for a user</summary>
        /// <param name="id">The identifier for the user whose schedule is to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given schedules from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("deleteall")]
        public IHttpActionResult Delete()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var idInt = Int32.Parse(id);
            var result = _scheduleService.DeleteAllForID(idInt);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}