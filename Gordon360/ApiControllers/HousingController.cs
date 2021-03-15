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
            var result = false; // This feature is not yet implemented
            return Ok(result);
        }

        /// <summary>Get apartment application ID number of currently logged in user if that user is on an existing application</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment")]
        public IHttpActionResult GetApplicationID()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string editorId = _accountService.GetAccountByUsername(username).GordonID;
            string sessionId = Helpers.GetCurrentSession().SessionCode;

            var result = _housingService.GetApplicationID(editorId, sessionId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
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
            //var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;

            string editorId = _accountService.GetAccountByUsername(username).GordonID;
            string sessionId = Helpers.GetCurrentSession().SessionCode;

            var result = _housingService.GetApplicationID(editorId, sessionId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// save application
        /// </summary>
        /// <returns>The application ID number of the saved application</returns>
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

            int apartAppId = apartmentAppDetails.AprtAppID; // The apartAppId is set to -1 if an existing application ID was not yet known by the frontend
            string[] applicantIds = new string[apartmentAppDetails.Applicants.Length];
            for(int i = 0; i < apartmentAppDetails.Applicants.Length; i++){
                applicantIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }

            int result = _housingService.SaveApplication(apartAppId, editorId, sessionId, applicantIds);

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

            bool result = _housingService.ChangeApplicationEditor(apartAppId, editorId, newEditorId);

            return Ok(result);
        }

        /// <summary>
        /// Create a string a string from the combination of AA_ApartementApplications and AA_Applicants Tables
        /// and returns it to the frontend, so that it can convert it into a csv file.
        /// </summary>
        /// <returns>An http result with the csv string from the CreateCSV service</returns>
        [HttpGet]
        [Route("apartment/csv")]
        //[StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult CreateCSV()
        {
            string result = _housingService.CreateCSV();
            return Created("Result of CSV creation: ", result);
        }

        /// <summary>Get apartment application info for a given application ID number</summary>
        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <returns>Object of type ApartmentAppViewModel</returns>
        [HttpGet]
        [Route("apartment/load/{applicationID}")]
        //[StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult GetApartmentApplication(string applicationID)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(applicationID))
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
            var result = false; // This feature is not yet implemented
            return Ok(result);
        }
    }
}
