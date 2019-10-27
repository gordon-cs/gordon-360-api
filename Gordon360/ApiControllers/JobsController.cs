using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Static.Methods;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/jobs")]
    public class JobsController : ApiController
    {
        public JobsController()
        {
            IUnitOfWork _uniteOfWork = new UnitOfWork();
        }

        /// <summary>
        /// This is just an end-to-end proof of concept.
        /// It will return Dr. Tuck's soul.
        /// <returns> Dr. Tuck's soul </returns>
        /// </summary>
        [HttpGet]
        [Route("hello-world")]
        public IHttpActionResult GetTestData()
        {
            return Ok("Hello World! We connected to the back-end.");
        }
    }
}
