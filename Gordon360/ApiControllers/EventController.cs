using System.Security.Claims;
using System.Linq;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;

namespace Gordon360.ApiControllers
{
    [CustomExceptionFilter]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        IEventService _eventService;
        public EventController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _eventService = new EventService(unitOfWork);
        }

        [Authorize]
        [HttpGet]
        [Route("attended/{term}")]
        public ActionResult<IEnumerable<AttendedEventViewModel>> GetEventsByTerm(string term)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(authenticatedUserUsername) || string.IsNullOrWhiteSpace(term))
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
        [Authorize]
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllEvents()
        {

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

            var result = _eventService.GetAllEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [Authorize]
        [HttpGet]
        [Route("claw")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllChapelEvents()
        {

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

            var result = _eventService.GetCLAWEvents();


            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [HttpGet]
        [Route("public")]
        public ActionResult<IEnumerable<EventViewModel>> GetAllPublicEvents()
        {
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

            var result = _eventService.GetPublicEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }

}

