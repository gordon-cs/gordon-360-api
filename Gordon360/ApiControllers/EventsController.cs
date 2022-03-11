using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Gordon360.ApiControllers
{
    public class EventsController : GordonControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(CCTContext context)
        {
            _eventService = new EventService(context);
        }

        [HttpGet]
        [Route("chapel/{term}")]
        public ActionResult<IEnumerable<AttendedEventViewModel>> GetEventsByTerm(string term)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            var result = _eventService.GetEventsForStudentByTerm(authenticatedUserUsername, term);

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
        [Route("25Live/All")]
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
        [Route("25Live/CLAW")]
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
        [Route("25Live/Public")]
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

