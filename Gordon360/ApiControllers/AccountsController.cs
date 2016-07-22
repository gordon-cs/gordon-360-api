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
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        IAccountService _accountService;
        public AccountsController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _accountService = new AccountService(unitOfWork);
        }
        // GET: api/Accounts
        [HttpGet]
        [Route("email/{email}")]
        public IHttpActionResult GetByAccountEmail(string email)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(email))
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

            var result = _accountService.GetAccountByEmail(email);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }

  
    
}
