using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System.Threading.Tasks;
using Gordon360.Models;
using System.Web;
using System.Diagnostics;
using Gordon360.Providers;
using System.IO;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;
using System.Security.Claims;
using System.Net.Http.Headers;
using Gordon360.Static.Data;

namespace Gordon360.ApiControllers
{
    [RoutePrefix("api/dining")]
    [CustomExceptionFilter]
    [Authorize]
    public class DiningController : ApiController
    {
        public IDiningService _diningService;
        public DiningController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _diningService = new DiningService(_unitOfWork);
        }

        /// <summary>
        ///  Gets information about student's dining plan and balance
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <param name="sessionCode">Current session code</param>
        /// <returns>A DiningInfo object</returns>
        [HttpGet]
        [Route("{id}/{sessionCode}")]
        public IHttpActionResult Get(int id, string sessionCode)
        {
            if (!ModelState.IsValid)
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

            var result = _diningService.GetDiningPlanInfo(id, sessionCode);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}