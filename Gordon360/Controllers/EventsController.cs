﻿using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : GordonControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(CCTContext context, IMemoryCache cache)
        {
            _eventService = new EventService(context, cache);
        }

        [HttpGet]
        [Route("attended/{term}")]
        public async Task<ActionResult<IEnumerable<AttendedEventViewModel>>> GetEventsByTermAsync(string term)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);

            var result = await _eventService.GetEventsForStudentByTermAsync(authenticatedUserUsername, term);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// This makes use of our cached request to 25Live, which stores AllEvents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllEvents()
        {
            var result = _eventService.GetAllEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [HttpGet]
        [Route("claw")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllChapelEvents()
        {
            var result = _eventService.GetCLAWEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("public")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllPublicEvents()
        {
            var result = _eventService.GetPublicEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }

}

