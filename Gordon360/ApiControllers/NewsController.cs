using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
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

        /** Get all approved student news entries not yet expired, filtering
         * out the expired by comparing 2 weeks past date entered to current date
         */
        [HttpGet]
        [Route("not-expired")]
        public IHttpActionResult GetNotExpired()
        {
            return Ok();
        }

        /** Get all new and approved student news entries,
         * checking novelty by comparing an entry's date entered to 10am on the current day
         */
        [HttpGet]
        [Route("new")]
        public IHttpActionResult GetNew()
        {
            return Ok();
        }



        
    }
}