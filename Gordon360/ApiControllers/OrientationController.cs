using System;
using System.Security.Claims;
using System.Linq;
using Gordon360.Static.Data;
using Gordon360.Static.Names;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.ApiControllers

{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/orientation")]
    public class OrientationController : ApiController
    {
        private IRoleCheckingService _roleCheckingService;
        private IAccountService _accountService;

        public OrientationController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _accountService = new AccountService(_unitOfWork);
        }

        // GET: Profile Photo Status
        // Uses the Account Service method to retrieve Account information
        [HttpGet]
        [Route("photo/{id}")]
        public IHttpActionResult GetAccountByUsername(string username)
        {
            result = _accountService.GetAccountByUsername(username);

            if (result == null)
            {
                return NotFound();
            }
            
            final = result.primary_photo;

            return Ok(final);
        }
    }
}
