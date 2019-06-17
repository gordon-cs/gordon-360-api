﻿using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/vpscore")]
    [CustomExceptionFilter]
    [Authorize]
    public class VictoryPromiseController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IVictoryPromiseService _victoryPromiseService;

        public VictoryPromiseController()
        {
            var _unitOfWork = new UnitOfWork();
            _victoryPromiseService = new VictoryPromiseService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }
        public VictoryPromiseController(IVictoryPromiseService victoryPromiseService)
        {
            _victoryPromiseService = victoryPromiseService;
        }

        /// <summary>
        ///  Gets current victory promise scores
        /// </summary>
        /// <returns>A VP object object</returns>
        [HttpGet]
        [Route("")]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            //var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            //var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            //var id = _accountService.GetAccountByUsername(username).GordonID;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var result = _victoryPromiseService.GetVPScores(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
        }
        }
}