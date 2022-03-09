using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.AuthorizationFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Gordon360.Database.CCT;
using Gordon360.Models.CCT;
using System.Threading.Tasks;

namespace Gordon360.Controllers.Api
{
    [Route("api/housing")]
    [Authorize]
    [CustomExceptionFilter]
    public class HousingController : ControllerBase
    {
        private readonly IHousingService _housingService;
        private readonly IAccountService _accountService;
        private readonly IAdministratorService _administratorService;
        private readonly IProfileService _profileService;

        public HousingController(CCTContext context)
        {
            _housingService = new HousingService(context);
            _accountService = new AccountService(context);
            _administratorService = new AdministratorService(context);
            _profileService = new ProfileService(context);
        }

        /// <summary>
        /// Check if the currently logged in user is authorized to view the housing admin page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("admin")]
        public ActionResult<bool> CheckIfHousingAdmin()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            string userID = _accountService.GetAccountByUsername(authenticatedUserUsername).GordonID;

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
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING_ADMIN)]
        public ActionResult<bool> AddHousingAdmin(string id)
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
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.HOUSING_ADMIN)]
        public ActionResult<bool> RemoveHousingAdmin(string id)
        {
            bool result = _housingService.RemoveHousingAdmin(id);
            return Ok(result);
        }

        /// <summary>
        /// Delete an application (and consequently all rows that reference it)
        /// </summary>
        /// <param name="applicationID"> The id of the application to remove </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apartment/applications/{applicationID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.HOUSING)]
        public ActionResult<bool> DeleteApplication(int applicationID)
        {
            bool result = _housingService.DeleteApplication(applicationID);
            return Ok(result);
        }


        /// <summary>
        /// Get a list of the apartment-style halls
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("halls/apartments")]
        public ActionResult<string[]> GetApartmentHalls()
        {
            var result = _housingService.GetAllApartmentHalls();
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
        /// Get apartment application ID number of currently logged in user if that user is on an existing application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apartment")]
        public async Task<ActionResult<int?>> GetApplicationID()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            string sessionID = (await Helpers.GetCurrentSession()).SessionCode;

            int? result = _housingService.GetApplicationID(authenticatedUserUsername, sessionID);
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
        public async Task<ActionResult<int?>> GetUserApplicationID(string username)
        {
            string sessionID = (await Helpers.GetCurrentSession()).SessionCode;

            int? result = _housingService.GetApplicationID(username, sessionID);
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
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.HOUSING)]
        public async Task<ActionResult<int>> SaveApplication([FromBody] ApartmentApplicationViewModel applicationDetails)
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

            string sessionID = (await Helpers.GetCurrentSession()).SessionCode;

            string editorUsername = applicationDetails?.EditorProfile?.AD_Username ?? applicationDetails?.EditorUsername;

            var apartmentApplicants = applicationDetails.Applicants;
            foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
            {
                if (applicant.Profile == null)
                {
                    applicant.Profile = _profileService.GetStudentProfileByUsername(applicant.Username);
                }
            }

            int result = _housingService.SaveApplication(sessionID, editorUsername, apartmentApplicants, applicationDetails.ApartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// update existing application (Differentiated by HttpPut instead of HttpPost)
        /// </summary>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        [HttpPut]
        [Route("apartment/applications/{applicationID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
        public async Task<ActionResult<int>> EditApplication(int applicationID, [FromBody] ApartmentApplicationViewModel applicationDetails)
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
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            string sessionID = (await Helpers.GetCurrentSession()).SessionCode;

            string newEditorUsername = applicationDetails?.EditorProfile?.AD_Username ?? applicationDetails?.EditorUsername;

            var newApartmentApplicants = applicationDetails.Applicants;
            foreach (ApartmentApplicantViewModel applicant in newApartmentApplicants)
            {
                if (applicant.Profile == null)
                {
                    applicant.Profile = _profileService.GetStudentProfileByUsername(applicant.Username);
                }
            }

            int result = _housingService.EditApplication(authenticatedUserUsername, sessionID, applicationID, newEditorUsername, newApartmentApplicants, applicationDetails.ApartmentChoices);

            return Created("Status of application saving: ", result);
        }

        /// <summary>
        /// change the editor (i.e. primary applicant) of the application
        /// </summary>
        /// <returns>The result of changing the editor</returns>
        [HttpPut]
        [Route("apartment/applications/{applicationID}/editor")]
        public ActionResult<bool> ChangeEditor(int applicationID, [FromBody] ApartmentApplicationViewModel applicationDetails)
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
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            string newEditorUsername = applicationDetails?.EditorProfile?.AD_Username ?? applicationDetails?.EditorUsername;

            bool result = _housingService.ChangeApplicationEditor(authenticatedUserUsername, applicationID, newEditorUsername);

            return Ok(result);
        }

        /// <summary>
        /// change the date an application was submitted
        /// (changes it from null the first time they submit)
        /// </summary>
        /// <returns>The result of changing the date submitted</returns>
        [HttpPut]
        [Route("apartment/applications/{applicationID}/submit")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
        public ActionResult<bool> ChangeApplicationDateSubmitted(int applicationID)
        {
            bool result = _housingService.ChangeApplicationDateSubmitted(applicationID);
            return Ok(result);
        }

        /// <summary>Get apartment application info for a given application ID number</summary>
        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <returns>Object of type ApartmentAppViewModel</returns>
        [HttpGet]
        [Route("apartment/applications/{applicationID}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.HOUSING)]
        public ActionResult<ApartmentApplicationViewModel> GetApartmentApplication(int applicationID)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserUsername = User.FindFirst(ClaimTypes.Name).Value;

            string userID = _accountService.GetAccountByUsername(authenticatedUserUsername).GordonID;

            bool isAdmin = false;

            try
            {
                ADMIN adminModel = _administratorService.Get(userID);
                isAdmin = (adminModel != null);
            }
            catch
            {
                isAdmin = _housingService.CheckIfHousingAdmin(userID);
            }

            ApartmentApplicationViewModel result = _housingService.GetApartmentApplication(applicationID, isAdmin);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>Get apartment application info for all applications if the current user is a housing admin</summary>
        /// <returns>Object of type ApartmentApplicationViewModel</returns>
        [HttpGet]
        [Route("admin/apartment/applications/")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.HOUSING)]
        public ActionResult<ApartmentApplicationViewModel[]> GetAllApartmentApplication()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            bool isAdmin = _housingService.CheckIfHousingAdmin(authenticatedUserIdString); // This line can be removed once the StateYourBusiness has been implemented
            if (isAdmin) // This line can be removed once the StateYourBusiness has been implemented
            {
                ApartmentApplicationViewModel[] result = _housingService.GetAllApartmentApplication();
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            else // This line can be removed once the StateYourBusiness has been implemented
            {
                throw new UnauthorizedAccessException("Unauthorized to view apartment applications"); // This line can be removed once the StateYourBusiness has been implemented
            }
        }
    }
}
