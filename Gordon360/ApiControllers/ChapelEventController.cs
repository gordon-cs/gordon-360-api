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
    [RoutePrefix("api/chapel_event")]
    public class ChapelEventController : ApiController
    {
        IChapelEventService _chapelService;
        public ChapelEventController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _chapelService = new ChapelEventService(unitOfWork);
        }
        // GET: api/Accounts
        [HttpGet]
        [Route("CHEventID/{CHEventID}")]
        public IHttpActionResult GetChapelEventByChapelEventID(string CHEventID)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(CHEventID))
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

            var result = _chapelService.GetChapelEventByChapelEventID(CHEventID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        [Route("Student/{ID}")]
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
        [Route("Student/{ID}/{term}")]
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

