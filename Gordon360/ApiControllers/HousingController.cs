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
        private ISessionService _sessionService;
        private IAccountService _accountService;
      




        public HousingController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _housingService = new HousingService(_unitOfWork);
            _sessionService = new SessionService(_unitOfWork);
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

        /// <summary>
        /// save application
        /// </summary>
        /// <returns>The result of submitting the application</returns>
        [HttpPost]
        [Route("save")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
        public IHttpActionResult SaveApplication(string username, string [] applicants)
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
            SessionViewModel currentSessionViewModel = null;
            string sessionId = currentSessionViewModel.SessionCode;
            string[] appIds = new string[applicants.Length];
            for(int i = 0; i <= applicants.Length; i++){
                appIds[i] = _accountService.GetAccountByUsername(username).GordonID;
            }
            _housingService.SaveApplication(username, sessionId, appIds);

            return Ok();

        }
    }
}
