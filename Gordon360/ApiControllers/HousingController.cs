using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Static.Methods;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;

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
        /// Check if the currently logged in user is authorized to view the housing admin page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("admin")]
        public IHttpActionResult CheckIfHousingAdmin()
        {
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;

            bool result = _housingService.CheckIfHousingAdmin(userID);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Add a user to the admin whitelist
        /// </summary>
        /// <param name="id"> The id of the user to add </param>
        /// <returns></returns>
        [HttpPost]
        [Route("admin/{id}")]
        public IHttpActionResult AddHousingAdmin(string id)
        {
            bool result = _housingService.AddHousingAdmin(id);
            return Ok(result);
        }

        /// <summary>
        /// Remove a user from the admin whitelist
        /// </summary>
        /// <param name="id"> The id of the user to remove </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("admin/{id}")]
        public IHttpActionResult RemoveHousingAdmin(string id)
        {
            bool result = _housingService.RemoveHousingAdmin(id);
            return Ok(result);
        }

        /// <summary>
        /// Get apartment application ID number of currently logged in user if that user is on an existing application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment")]
        public IHttpActionResult GetApplicationID()
        {
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string editorID = _accountService.GetAccountByUsername(username).GordonID;
            string sessionID = Helpers.GetCurrentSession().SessionCode;

            int? result = _housingService.GetApplicationID(editorID, sessionID);
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
        /// Get apartment application ID number for a user if that user is on an existing application
        /// </summary>
        /// <param name="username">username of the profile info</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment/{username}")]
        public IHttpActionResult GetUserApplicationID(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
            {
                string errors = "";
                foreach (System.Web.Http.ModelBinding.ModelState modelstate in ModelState.Values)
                {
                    foreach (System.Web.Http.ModelBinding.ModelError error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            string editorID = _accountService.GetAccountByUsername(username).GordonID;
            string sessionID = Helpers.GetCurrentSession().SessionCode;

            int? result = _housingService.GetApplicationID(editorID, sessionID);
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
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        [HttpPost]
        [Route("apartment/applications")]
        //[StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult SaveApplication([FromBody] ApartmentApplicationViewModel applicationDetails)
        {
            // Verify Input
            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach (System.Web.Http.ModelBinding.ModelState modelstate in ModelState.Values)
                {
                    foreach (System.Web.Http.ModelBinding.ModelError error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;

            string sessionID = Helpers.GetCurrentSession().SessionCode;

            string editorID = _accountService.GetAccountByUsername(applicationDetails.EditorUsername).GordonID;

            ApartmentApplicantViewModel[] apartmentApplicants = applicationDetails.Applicants;
            foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
            {
                applicant.StudentID = _accountService.GetAccountByUsername(applicant.Username).GordonID;
            }

            ApartmentChoiceViewModel[] apartmentChoices = applicationDetails.ApartmentChoices;

            int result = _housingService.SaveApplication(userID, sessionID, editorID, apartmentApplicants, apartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// update existing application (Differentiated by HttpPut instead of HttpPost)
        /// </summary>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        [HttpPut]
        [Route("apartment/applications/{applicationID}")]
        public IHttpActionResult EditApplication(int applicationID, [FromBody] ApartmentApplicationViewModel applicationDetails)
        {
            // Verify Input
            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach (System.Web.Http.ModelBinding.ModelState modelstate in ModelState.Values)
                {
                    foreach (System.Web.Http.ModelBinding.ModelError error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;

            string sessionID = Helpers.GetCurrentSession().SessionCode;

            if (applicationID == -1)
            {
                applicationID = applicationDetails.ApplicationID; // The apartAppID is set to -1 if an existing application ID was not yet known by the frontend
            }

            string newEditorUsername = applicationDetails.EditorUsername;
            string newEditorID = _accountService.GetAccountByUsername(newEditorUsername).GordonID;

            ApartmentApplicantViewModel[] newApartmentApplicants = applicationDetails.Applicants;
            foreach (ApartmentApplicantViewModel applicant in newApartmentApplicants)
            {
                applicant.StudentID = _accountService.GetAccountByUsername(applicant.Username).GordonID;
            }

            ApartmentChoiceViewModel[] newApartmentChoices = applicationDetails.ApartmentChoices;

            int result = _housingService.EditApplication(userID, sessionID, applicationID, newEditorID, newApartmentApplicants, newApartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// change the editor (i.e. primary applicant) of the application
        /// </summary>
        /// <returns>The result of changing the editor</returns>
        [HttpPut]
        [Route("apartment/applications/{applicationID}/{editorUsername}")]
        public IHttpActionResult ChangeEditor(int applicationID, string editorUsername) // [FromBody] ApartmentAppNewEditorViewModel newEditorDetails)
        {
            // Verify Input
            if (!ModelState.IsValid)
            {
                string errors = "";
                foreach (System.Web.Http.ModelBinding.ModelState modelstate in ModelState.Values)
                {
                    foreach (System.Web.Http.ModelBinding.ModelError error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;

            //int apartAppID = newEditorDetails.AprtAppID;
            //string newEditorUsername = newEditorDetails.EditorUsername;
            string newEditorID = _accountService.GetAccountByUsername(editorUsername).GordonID;

            bool result = _housingService.ChangeApplicationEditor(userID, applicationID, newEditorID);

            return Ok(result);
        }


        /// <summary>Get apartment application info for a given application ID number</summary>
        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <returns>Object of type ApartmentAppViewModel</returns>
        [HttpGet]
        [Route("apartment/applications/{applicationID}")]
        //[StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult GetApartmentApplication(int applicationID)
        {
            if (!ModelState.IsValid || applicationID != null)
            {
                string errors = "";
                foreach (System.Web.Http.ModelBinding.ModelState modelstate in ModelState.Values)
                {
                    foreach (System.Web.Http.ModelBinding.ModelError error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;
            string sessionID = Helpers.GetCurrentSession().SessionCode;

            int? storedApplicationID = _housingService.GetApplicationID(userID, sessionID);
            if (storedApplicationID == null)
            {
                return NotFound();
            }
            else if (storedApplicationID != applicationID)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            else
            {
                ApartmentApplicationViewModel result = _housingService.GetApartmentApplication(applicationID);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        /// <summary>Get apartment application info for all applications if the current user is a housing admin</summary>
        /// <returns>Object of type ApartmentAppViewModel</returns>
        [HttpGet]
        [Route("admin/apartment/applications/")]
        //[StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult GetAllApartmentApplication()
        {
            //get token data from context, username is the username of current logged in person
            ClaimsPrincipal authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            string username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            string userID = _accountService.GetAccountByUsername(username).GordonID;

            bool isAdmin = _housingService.CheckIfHousingAdmin(userID);
            if (isAdmin)
            {
                string sessionID = Helpers.GetCurrentSession().SessionCode;

                ApartmentApplicationViewModel[] result = _housingService.GetAllApartmentApplication(sessionID);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }
    }
}
