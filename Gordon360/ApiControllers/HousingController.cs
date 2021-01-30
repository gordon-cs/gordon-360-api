using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;

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

        /// <summary>
        /// save application
        /// </summary>
        /// <returns>The result of submitting the application</returns>
        [HttpPost]
        [Route("apartment/save")]
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

            string editorId = _accountService.GetAccountByUsername(username).GordonID;

            string sessionId = Helpers.GetCurrentSession().SessionCode;

            string[] applicantIds = new string[apartmentAppDetails.Applicants.Length];
            for(int i = 0; i < apartmentAppDetails.Applicants.Length; i++){
                applicantIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }

            ApartmentChoiceViewModel[] apartmentChoices = apartmentAppDetails.ApartmentChoices;

            int result = _housingService.SaveApplication(editorId, sessionId, applicantIds, apartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// edit existing application
        /// </summary>
        /// <returns>Returns true if all the queries succeeded, otherwise returns false</returns>
        [HttpPut]
        [Route("apartment/edit")]
        public IHttpActionResult EditApplication([FromBody] ApartmentAppViewModel apartmentAppDetails)
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

            string editorId = _accountService.GetAccountByUsername(username).GordonID;

            string sessionId = Helpers.GetCurrentSession().SessionCode;

            int apartAppId = apartmentAppDetails.AprtAppID; // The apartAppId is set to -1 if an existing application ID was not yet known by the frontend
            string[] newApplicantIds = new string[apartmentAppDetails.Applicants.Length];
            for (int i = 0; i < apartmentAppDetails.Applicants.Length; i++)
            {
                newApplicantIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }

            ApartmentChoiceViewModel[] newApartmentChoices = apartmentAppDetails.ApartmentChoices;

            bool result = _housingService.EditApplication(editorId, sessionId, apartAppId, newApplicantIds, newApartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// change the editor (i.e. primary applicant) of the application
        /// </summary>
        /// <returns>The result of changing the editor</returns>
        [HttpPost]
        [Route("apartment/change-editor")]
        public IHttpActionResult ChangeEditor([FromBody] ApartmentAppNewEditorViewModel newEditorDetails)
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

            string editorId = _accountService.GetAccountByUsername(username).GordonID;

            int apartAppId = newEditorDetails.AprtAppID;
            string newEditorUsername = newEditorDetails.Username;
            string newEditorId = _accountService.GetAccountByUsername(newEditorUsername).GordonID;

            bool result = _housingService.ChangeApplicationEditor(editorId, apartAppId, newEditorId);

            return Ok(result);
        }
    }
}
