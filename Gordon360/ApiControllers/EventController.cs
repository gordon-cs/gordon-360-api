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
    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        IChapelEventService _chapelService;
        public EventController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _chapelService = new EventService(unitOfWork);
        }
        
        [HttpGet]
        [Route("chapel/Student/{ID}")]
        public IHttpActionResult GetAllForStudent(string ID)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(ID))
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

            var result = _chapelService.GetAllForStudent(ID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        [Route("chapel/Student/{ID}/{term}")]
        public IHttpActionResult GetEventsForStudentByTerm(string ID, string term)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(ID) || string.IsNullOrWhiteSpace(term))
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

            var result = _chapelService.GetEventsForStudentByTerm(ID, term);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }




    }




}

