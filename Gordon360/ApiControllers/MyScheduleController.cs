using Gordon360.Services;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Database.CCT;

namespace Gordon360.Controllers.Api
{
    [Route("api/myschedule")]
    [CustomExceptionFilter]
    [Authorize]
    public class MyScheduleController : ControllerBase
    {
        //declare services we are going to use.
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;
        private readonly IRoleCheckingService _roleCheckingService;
        private readonly IScheduleControlService _scheduleControlService;
        private readonly IMyScheduleService _myScheduleService;

        public MyScheduleController(CCTContext context)
        {
            _myScheduleService = new MyScheduleService(context);
            _profileService = new ProfileService(context);
            _accountService = new AccountService(context);
            _roleCheckingService = new RoleCheckingService(context);
            _scheduleControlService = new ScheduleControlService(context);
        }

        public MyScheduleController(IMyScheduleService myScheduleService)
        {
            _myScheduleService = myScheduleService;
        }

        /// <summary>
        ///  Gets all custom events for a user
        /// </summary>
        /// <returns>A IEnumerable of custom events</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<MYSCHEDULE>> Get()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            object result = _myScheduleService.GetAllForID(authenticatedUserIdString);
            if (result == null)
            {
                return NotFound();
            }

            JArray jresult = JArray.FromObject(result);

            foreach (JObject elem in jresult)
            {
                elem.Property("GORDON_ID").Remove();
            }

            return Ok(jresult);
        }


        /// <summary>
        ///  Gets specific custom event for a user
        /// </summary>
        /// <returns>The requested custom event</returns>
        [HttpGet]
        [Route("event/{event_id}")]
        public ActionResult<MYSCHEDULE> GetByEventId(string event_Id)
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            object result = _myScheduleService.GetForID(event_Id, authenticatedUserIdString);
            if (result == null)
            {
                return NotFound();
            }

            JObject jresult = JObject.FromObject(result);

                jresult.Property("GORDON_ID").Remove();

            return Ok(jresult);
        }

        /// <summary>
        ///  Gets all myschedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of myschedule objects</returns>
        [HttpGet]
        [Route("{username}")]
        public ActionResult<IEnumerable<MYSCHEDULE>> Get(string username)
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
        public ActionResult<MYSCHEDULE> Post([FromBody] MYSCHEDULE mySchedule)
        {
            const int MAX = 50;
            DateTime localDate = DateTime.Now;

            // Verify Input
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

            // Check if maximum
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            object existingEvents = _myScheduleService.GetAllForID(authenticatedUserIdString);

            JArray jEvents = JArray.FromObject(existingEvents);

            if (jEvents.Count > MAX)
            {
                return Unauthorized();
            }


            var result = _myScheduleService.Add(mySchedule);

            if (result == null)
            {
                return NotFound();
            }


            _scheduleControlService.UpdateModifiedTimeStamp(authenticatedUserIdString, localDate);

            return Created("myschedule", mySchedule);
        }

        /// <summary>Delete an existing myschedule item</summary>
        /// <param name="event_id">The identifier for the myschedule to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given myschedule from the database</remarks>
        [HttpDelete]
        [Route("{event_id}")]
        public ActionResult<MYSCHEDULE> Delete(string event_id)
        {
            DateTime localDate = DateTime.Now;
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _myScheduleService.Delete(event_id, authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            _scheduleControlService.UpdateModifiedTimeStamp(authenticatedUserIdString, localDate);

            return Ok(result);
        }

        /// <summary>Update the existing myschedule in database</summary>
        /// <param name="mySchedule">The updated myschedule item containing all required and relevant information</param>
        /// <returns>Original schedule</returns>
        /// <remarks>Put a myschedule to the server to be updated</remarks>
        [HttpPut]
        [Route("")]
        public ActionResult<MYSCHEDULE> Put([FromBody] MYSCHEDULE mySchedule)
        {
            DateTime localDate = DateTime.Now;
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

            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            _scheduleControlService.UpdateModifiedTimeStamp(authenticatedUserIdString, localDate);

            return Ok(result);
        }

    }
}
