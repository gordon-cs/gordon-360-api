using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;


namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/housing")]
    [Authorize]
    [CustomExceptionFilter]
    public class HousingController : ApiController
    {
        private IHousingService _housingService;
        //private ISessionService _sessionService;
        private IAccountService _accountService;

        public HousingController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _housingService = new HousingService(_unitOfWork);
            //_sessionService = new SessionService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        /** Call the service that gets all student housing information
         */
        [HttpGet]
        [Route("apartmentInfo")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT)]
        public IHttpActionResult GetApartmentInfo()
        {
            var result = _housingService.GetAll();
            return Ok(result);
        }

        /// <summary>Check if the currently logged in user is authorized to view the housing staff page for applications</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("staff")]
        public IHttpActionResult CheckHousingStaff()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            //! Placeholder
            var result = "This feature is not yet implemented";
            return Ok(result);
        }

        /// <summary>Get apartment application ID number of currently logged in user if that user is on an existing application</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment")]
        public IHttpActionResult GetApartmentAppID()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            //! Placeholder
            var result = "This feature is not yet implemented";
            return Ok(result);
        }
        /// <summary>Get apartment application ID number for a user if that user is on an existing application</summary>
        /// <param name="username">username of the profile info</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment/{username}")]
        public IHttpActionResult GetUserApplicationID(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
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
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;

            //! Placeholder
            var result = "This feature is not yet implemented";
            return Ok(result);
        }

        /// <summary>
        /// save application
        /// </summary>
        /// <returns>The application ID number of the saved application</returns>
        [HttpPost]
        [Route("apartment/save")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
        public IHttpActionResult SaveApplication([FromBody] ApartmentAppViewModel apartmentAppDetails)
        {
            // Verify Input
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

            string sessionId = Helpers.GetCurrentSession().SessionCode;
            string[] appIds = new string[apartmentAppDetails.Applicants.Length];
            for(int i = 0; i <= apartmentAppDetails.Applicants.Length; i++){
                appIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }
            _housingService.SaveApplication(apartmentAppDetails.Username, sessionId, appIds);

            return Ok();

        }

        /// <summary>Get apartment application info for a given application ID number</summary>
        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <returns>Object of type ApartmentAppViewModel</returns>
        [HttpGet]
        [Route("apartment/{applicationID}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)]
        public IHttpActionResult GetAppartmentApplication(string applicationID)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
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

            //! Placeholder
            var result = "This feature is not yet implemented";
            return Ok(result);
        }
    }
}
