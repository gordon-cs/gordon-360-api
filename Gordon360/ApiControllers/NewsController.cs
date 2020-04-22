using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        private INewsService _newsService;

        // Constructor
        public NewsController()
        {
            // Connect to service through which data (from the database) can be accessed 
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _newsService = new NewsService(_unitOfWork);
        }

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }


        /** Get all approved student news entries for the specified session
         * id: the identifier of a specific session
         */
        [HttpGet]
        [Route("session/{id}")]
        public IHttpActionResult GetNewsForSession(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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

            var result = _activityService.GetNewsForSession(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }
    }
}