using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.ApiControllers
{
    [RoutePrefix("api/log")]
    [Authorize]
    [CustomExceptionFilter]
    public class ErrorLogController : ApiController
    {

        private IErrorLogService _errorLogService;

        public ErrorLogController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _errorLogService = new ErrorLogService(_unitOfWork);
        }
        public ErrorLogController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }


        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="error_log">The error log containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new error_log to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name = "error_log")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.ERROR_LOG)]
        public IHttpActionResult Post([FromBody] ERROR_LOG error_log)
        {
            System.Diagnostics.Debug.WriteLine("Started post error log " + error_log);

            if (!ModelState.IsValid || error_log == null)
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

            var result = _errorLogService.Add(error_log);

            _errorLogService.Log("Testing String Error Message");


            return Created("error log", result);

        }
    }
}