using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using System.Web.Http;
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

            int apartAppId = apartmentAppDetails.AprtAppID; // Set the apartAppId to -1 to indicate that an application ID was not passed by the frontend
            string editor = _accountService.GetAccountByUsername(apartmentAppDetails.Username).GordonID;
            string sessionId = Helpers.GetCurrentSession().SessionCode;
            string[] applicantIds = new string[apartmentAppDetails.Applicants.Length];
            for(int i = 0; i < apartmentAppDetails.Applicants.Length; i++){
                applicantIds[i] = _accountService.GetAccountByUsername(apartmentAppDetails.Applicants[i]).GordonID;
            }

            int result = _housingService.SaveApplication(apartAppId, editor, sessionId, applicantIds);

            return Created("Status of application saving: ", result);

        }

        /// <summary>
        /// Export the SQL data into CSV file
        /// </summary>
        /// <returns>CSV file with an exported data</returns>
        [HttpPost]
        [Route("ExportCSV")]
        //[StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)] we need to actually add HOUSING to stateYourBusiness if we do this
        public IHttpActionResult ExportCSV([FromBody] ApartmentAppViewModel apartmentAppDetails)
        {
            return 0;
        }
    }
}
