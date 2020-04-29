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

        /** Get all approved student news entries not yet expired
         */
        [HttpGet]
        [Route("not-expired")]
        public IHttpActionResult GetNotExpired()
        {
            return Ok();
        }

        /** Get all approved student news entries for the current day that are not expired
         * date: today's date
         */
        [HttpGet]
        [Route("day/{date}")]
        public IHttpActionResult GetForDay(string date)
        {
            return Ok();
        }



        
    }
}