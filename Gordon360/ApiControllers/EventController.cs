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
        IChapelEventService _chapelService;
        public EventController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _chapelService = new EventService(unitOfWork);
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

            var result = _chapelService.GetAllForStudent(user_name);

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

            var result = _chapelService.GetEventsForStudentByTerm(user_name, term);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }




    }




}

