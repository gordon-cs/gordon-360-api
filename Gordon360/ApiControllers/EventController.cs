using System;
using System.Linq;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/events")]
    public class EventController : ApiController
    {
        IEventService _eventService;
        public EventController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _eventService = new EventService(unitOfWork);
        }
        
        [HttpGet]
        [Route("chapel/{user_name}")]
        public IHttpActionResult GetAllForStudent(string user_name)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(user_name))
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

            var result = _eventService.GetAllForStudent(user_name);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("chapel/{user_name}/{term}")]
        public IHttpActionResult GetEventsForStudentByTerm(string user_name, string term)
        {
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

        [HttpGet]
        [Route("25Live/type/{Type_ID}")]
        public IHttpActionResult GetEventsByType(string Type_ID)
        {
            // Two important checks: make sure the event_or_type_id does not contain any letters, and make sure the type is a single letter
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Type_ID) )
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                // Throw errors for invalid route
     
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _eventService.GetSpecificEvents(Type_ID, "t");

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        [HttpGet]
        [Route("25Live/{Event_ID}")]
        public IHttpActionResult GetEventsByID(string Event_ID)
        {
            // Two important checks: make sure the event_or_type_id does not contain any letters, and make sure the type is a single letter
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Event_ID))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                // Throw errors for invalid route

                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _eventService.GetSpecificEvents(Event_ID, "m");

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }


        [HttpGet]
        [Route("25Live/All")]
        public IHttpActionResult GetAllEvents()
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

            var result = _eventService.GetAllEvents(Static.Data.Data.AllEvents);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

    }




}

