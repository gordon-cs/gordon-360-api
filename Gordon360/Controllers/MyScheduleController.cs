using Gordon360.Database.CCT;
using Gordon360.Models.CCT;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class MyScheduleController : GordonControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IScheduleControlService _scheduleControlService;
        private readonly IMyScheduleService _myScheduleService;

        public MyScheduleController(CCTContext context)
        {
            _myScheduleService = new MyScheduleService(context);
            _accountService = new AccountService(context);
            _scheduleControlService = new ScheduleControlService(context);
        }

        /// <summary>
        ///  Gets all custom events for a user
        /// </summary>
        /// <returns>A IEnumerable of custom events</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<MYSCHEDULE>> Get()
        {
            var authenticatedUserIdString = AuthUtils.GetUsername(User);

            object result = _myScheduleService.GetAllForUser(authenticatedUserIdString);
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
            var authenticatedUserIdString = AuthUtils.GetUsername(User);

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
            var result = _myScheduleService.GetAllForUser(username);
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

            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            object existingEvents = _myScheduleService.GetAllForUser(authenticatedUserUsername);

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


            _scheduleControlService.UpdateModifiedTimeStampAsync(authenticatedUserUsername, localDate);

            return Created("myschedule", mySchedule);
        }

        /// <summary>Delete an existing myschedule item</summary>
        /// <param name="eventID">The identifier for the myschedule to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given myschedule from the database</remarks>
        [HttpDelete]
        [Route("{event_id}")]
        public ActionResult<MYSCHEDULE> Delete(string eventID)
        {
            DateTime localDate = DateTime.Now;
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var result = _myScheduleService.Delete(eventID, authenticatedUserUsername);

            if (result == null)
            {
                return NotFound();
            }

            _scheduleControlService.UpdateModifiedTimeStampAsync(authenticatedUserUsername, localDate);

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

            var result = _myScheduleService.Update(mySchedule);

            if (result == null)
            {
                return NotFound();
            }

            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            _scheduleControlService.UpdateModifiedTimeStampAsync(authenticatedUserUsername, localDate);

            return Ok(result);
        }

    }
}
