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
    [RoutePrefix("api/dining")]
    public class DiningController : ApiController
    {
        IDiningService _diningService;
        public DiningController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _diningService = new DiningService(unitOfWork);
        }
        // GET: api/dining
        [HttpGet]
        [Route("balance")]
        public IHttpActionResult getBalance()
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

            var result = _diningService.GetBalanceAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }

  
    
}
