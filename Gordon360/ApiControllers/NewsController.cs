using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
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

        private void catchBadInput()
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
        }

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

        /** Call the service that gets all approved student news entries not yet expired, filtering
         * out the expired by comparing 2 weeks past date entered to current date
         */
        [HttpGet]
        [Route("not-expired")]
        public IHttpActionResult GetNotExpired()
        {
            var result = _newsService.GetNewsNotExpired();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets all new and approved student news entries
         * which have not already expired,
         * checking novelty by comparing an entry's date entered to 10am on the previous day
         */
        [HttpGet]
        [Route("new")]
        public IHttpActionResult GetNew()
        {
            var result = _newsService.GetNewsNew();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets the list of categories
         */
        [HttpGet]
        [Route("categories")]
        public IHttpActionResult GetCategories()
        {
            var result = _newsService.GetNewsCategories();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Create a new news item to be added to the database
         */
        [HttpPost]
        [Route("", Name="News")] //?
        public IHttpActionResult Post([FromBody] StudentNews newsItem)
        {
            if (!ModelState.IsValid /*|| validate input from newsItem here??*/ )
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
            
            var result = _newsService.Add(newsItem);
            if (result == null)
            {
                return NotFound();
            }
            return Created("News", result);
        }


    }
}