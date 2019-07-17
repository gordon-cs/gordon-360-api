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
    [RoutePrefix("api/myschedule")]
    [CustomExceptionFilter]
    [Authorize]
    public class MyScheduleController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IMyScheduleService _myScheduleService;

        public MyScheduleController()
        {
            var _unitOfWork = new UnitOfWork();
            _myScheduleService = new MyScheduleService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public MyScheduleController(IMyScheduleService myScheduleService)
        {
            _myScheduleService = myScheduleService;
        }

        /// <summary>
        ///  Gets all myschedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of myschedule objects</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _myScheduleService.GetAllForID(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        /// <summary>
        ///  Gets specific myschedule object for a user
        /// </summary>
        /// <returns>The requested myschedule object</returns>
        [HttpGet]
        [Route("event/{event_id}")]
        public IHttpActionResult GetByEventId(string event_Id)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _myScheduleService.GetForID(event_Id, id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        ///  Gets all myschedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of myschedule objects</returns>
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            //probably needs privacy stuff like ProfilesController and service
            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _myScheduleService.GetAllForID(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        /// <summary>Create a new myschedule to be added to database</summary>
        /// <param name="mySchedule">The myschedule item containing all required and relevant information</param>
        /// <returns>Created schedule</returns>
        /// <remarks>Posts a new myschedule to the server to be added into the database</remarks>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] MYSCHEDULE mySchedule)
        {
            if (!ModelState.IsValid || mySchedule == null)
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

            var result = _myScheduleService.Add(mySchedule);

            if (result == null)
            {
                return NotFound();
            }

            return Created("myschedule", mySchedule);
        }

        /// <summary>Delete an existing myschedule item</summary>
        /// <param name="event_id">The identifier for the myschedule to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given myschedule from the database</remarks>
        [HttpDelete]
        [Route("{event_id}")]
        public IHttpActionResult Delete(string event_id)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;
            var result = _myScheduleService.Delete(event_id, id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Update the existing myschedule in database</summary>
        /// <param name="mySchedule">The updated myschedule item containing all required and relevant information</param>
        /// <returns>Original schedule</returns>
        /// <remarks>Put a myschedule to the server to be updated</remarks>
        [HttpPut]
        [Route("")]
        public IHttpActionResult Put([FromBody] MYSCHEDULE mySchedule)
        {
            if (!ModelState.IsValid || mySchedule == null)
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

            var result = _myScheduleService.Update(mySchedule);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}