using System;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
using Gordon360.Exceptions.CustomExceptions;
namespace Gordon360.ApiControllers
{
        [RoutePrefix("api/dm")]
        [CustomExceptionFilter]
        [Authorize]
        public class DirectMessageController : ApiController
        {
            private IProfileService _profileService;
            private IAccountService _accountService;
            private IRoleCheckingService _roleCheckingService;

            private IDirectMessageService _DirectMessageService;

            public DirectMessageController()
            {
                var _unitOfWork = new UnitOfWork();
                _DirectMessageService = new DirectMessageService();
                _profileService = new ProfileService(_unitOfWork);
                _accountService = new AccountService(_unitOfWork);
                _roleCheckingService = new RoleCheckingService(_unitOfWork);
            }

            public DirectMessageController(IDirectMessageService DirectMessageService)
            {
            _DirectMessageService = DirectMessageService;
            }

            /// <summary>
            ///  returns hello world example
            /// </summary>
            /// <returns>string and date</returns>
            [HttpGet]
            [Route("")]
            public IHttpActionResult Get()
            {
                DateTime currentTime = DateTime.Now;
                var result = "hello world I'm coming from the back end at: " + currentTime;

                return Ok(result);
            }

        }
}