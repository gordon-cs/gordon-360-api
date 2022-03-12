using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;
using Gordon360.Services.ComplexQueries;
using Gordon360.Services;
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/update")]
    public class UpdateController : ApiController
    {
        private IUpdateService _updateservice;
        private IAccountService _accountService;


        public UpdateController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _updateservice = new UpdateService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        private int GetCurrentUserID()
        {
            int userID = -1;
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            string id = _accountService.GetAccountByUsername(username).GordonID;
            userID = Convert.ToInt32(id);

            return userID;
        }

        [HttpPost]
        [Route("updateRequest")]
        public IHttpActionResult SendUpdateRequest([FromBody] EmailContentViewModel email)
        {
            int userID = GetCurrentUserID();
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
            _updateservice.SendUpdateRequest(userID, email.Content);
            return Ok();
        }
    }
}