using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class LogController : GordonControllerBase
    {
        private readonly IErrorLogService _errorLogService;

        public LogController(CCTContext context)
        {
            _errorLogService = new ErrorLogService(context);
        }
        public LogController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }

        /// <summary>Create a new error log item to be added to database</summary>
        /// <param name="error_message">The error message containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new message to the service to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name = "error_log")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.ERROR_LOG)]
        public ActionResult<ERROR_LOG> Post([FromBody] string error_message)
        {
            if (error_message == null)
            {
                throw new BadInputException();
            }

            var result = _errorLogService.Log(error_message);

            return Created("error log", result);
        }


        /// <summary>Create a new error log item to be added to database</summary>
        /// <param name="error_log">The error log containing the ERROR_TIME, and the LOG_MESSAGE</param>
        /// <returns></returns>
        /// <remarks>Posts a new error_log to the server to be added into the database. Useful if you want to input the datetime in the front end for greater accuracy</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("add", Name = "error_add")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.ERROR_LOG)]
        public ActionResult<ERROR_LOG> Post([FromBody] ERROR_LOG error_log)
        {
            var result = _errorLogService.Add(error_log);

            return Created("error log", result);

        }
    }
}
