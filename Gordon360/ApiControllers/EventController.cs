using System.Security.Claims;
using System.Linq;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.ApiControllers
{
    [CustomExceptionFilter]
    [RoutePrefix("api/events")]
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
        public IHttpActionResult GetEventsByTerm(string term)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var user_name = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(user_name) || string.IsNullOrWhiteSpace(term))
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

            var result = _eventService.GetEventsForStudentByTerm(user_name, term);

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
        public IHttpActionResult GetAllEvents()
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
        public IHttpActionResult GetAllChapelEvents()
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
        public IHttpActionResult GetAllPublicEvents()
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



        [Authorize]
        [HttpGet]
        [Route("chapel/{term}")]
        public IHttpActionResult DEPRECATED_GetEventsByTerm(string term)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var user_name = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(user_name) || string.IsNullOrWhiteSpace(term))
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

            var result = _eventService.DEPRECATED_GetEventsForStudentByTerm(user_name, term);

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
        [Route("25Live/All")]
        public IHttpActionResult DEPRECATED_GetAllEvents()
        {

            if (!ModelState.IsValid )
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

            var result = _eventService.DEPRECATED_GetAllEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [Authorize]
        [HttpGet]
        [Route("25Live/CLAW")]
        public IHttpActionResult DEPRECATED_GetAllChapelEvents()
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

            var result = _eventService.DEPRECATED_GetCLAWEvents();


            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [HttpGet]
        [Route("25Live/Public")]
        public IHttpActionResult DEPRECATED_GetAllPublicEvents()
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

            var result = _eventService.DEPRECATED_GetPublicEvents();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }

}

