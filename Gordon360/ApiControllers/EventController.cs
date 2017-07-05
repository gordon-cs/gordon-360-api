using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        [Route("chapel/Student/{user_name}")]
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
        [Route("chapel/Student/{user_name}/{term}")]
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
        [Route("25Live/{Event_OR_Type_ID}/{type}")]
        public IHttpActionResult GetEvents(string Event_OR_Type_ID, string type)
        {
            // Two important checks: make sure the event_or_type_id does not contain any letters, and make sure the type is a single letter
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Event_OR_Type_ID) || string.IsNullOrWhiteSpace(type) || 
                type.Length > 1 || !Event_OR_Type_ID.Any( x=> !char.IsLetter(x)))
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
                if (errors == "") {
                    if (type.Length > 1) {
                        throw new Exception("Invalid type!");
                    }
                    else if (!Event_OR_Type_ID.Any(x => !char.IsLetter(x))){
                        throw new Exception("Invalid event identifyer!");
                    }
                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _eventService.GetEvents(Event_OR_Type_ID, type);

            if (result == null)
            {
                return NotFound();
            }


            return Ok(result);

        }




    }




}

