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
        [Route("updateRequest/")]
        public HttpResponseMessage updateUserInfo([FromBody] UpdateAlumniViewModel alumniInfo)
        {
            IEnumerable<UpdateAlumniViewModel> result = null;

            int userID = GetCurrentUserID();
            try
            {
                result = _updateservice.updateInfo(userID, alumniInfo.EMAIL, alumniInfo.HOME_PHONE, alumniInfo.MOBILE_PHONE, alumniInfo.ADDRESS_1, alumniInfo.ADDRESS_2, alumniInfo.CITY, alumniInfo.STATE);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}