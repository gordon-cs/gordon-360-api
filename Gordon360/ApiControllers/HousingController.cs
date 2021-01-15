using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;
using System;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/housing")]
    [Authorize]
    [CustomExceptionFilter]
    public class HousingController : ApiController
    {
        private IHousingService _housingService;
        private IAccountService _accountService;

        public HousingController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _housingService = new HousingService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

        /** Call the service that gets all student housing information
         */
        /*
        [HttpGet]
        [Route("apartmentInfo")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.STUDENT)]
        public IHttpActionResult GetApartmentInfo()
        {
            var result = _housingService.GetAll();
            return Ok(result);
        }
        */

        /// <summary>
        /// save application
        /// </summary>
        /// <returns>The result of submitting the application</returns>
        [HttpPost]
        [Route("save")]
        //[StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
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
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string modifierId = _accountService.GetAccountByUsername(username).GordonID;

            string sessionId = Helpers.GetCurrentSession().SessionCode;

            int apartAppId = apartmentAppDetails.AprtAppID; // The apartAppId is set to -1 if an existing application ID was not yet known by the frontend
            // string modifierId = _accountService.GetAccountByUsername(apartmentAppDetails.Username).GordonID;
            string[] applicantIds = new string[apartmentAppDetails.Applicants.Length];
            for(int i = 0; i < apartmentAppDetails.Applicants.Length; i++){
                applicantIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }

            int result = _housingService.SaveApplication(apartAppId, modifierId, sessionId, applicantIds);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// change the modifier (i.e. primary applicant) of the application
        /// </summary>
        /// <param name="applicationID"> The application ID number of the apartment application to be modified </param>
        /// <param name="newModifierUsername"> The username of the student who will be set as the editor of this apartment application </param>
        /// <returns>The result of changing the modifier</returns>
        [HttpPost]
        [Route("change-modifier/{applicationID}/{newModifierUsername}")]
        public IHttpActionResult ChangeModifier(int applicationID, string newModifierUsername)
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
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string modifierId = _accountService.GetAccountByUsername(username).GordonID;

            string newModifierId = _accountService.GetAccountByUsername(newModifierUsername).GordonID;

            bool result = _housingService.ChangeApplicationModifier(applicationID, modifierId, newModifierId);

            return Ok(result);
        }
    }
}
