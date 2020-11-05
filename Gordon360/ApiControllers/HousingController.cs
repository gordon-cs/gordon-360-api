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
        private IUnitOfWork _unitOfWork;
        private IHousingService _housingService;
        private IAccountService _accountService;


        public HousingController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _housingService = new HousingService(_unitOfWork);
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
        /// Submit shifts
        /// </summary>
        /// <returns>The result of submitting the shifts</returns>
        [HttpPost]
        [Route("submit")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.STUDENT)]
        public IHttpActionResult SubmitHousingAppForUser([FromBody] IEnumerable<HousingAppToSubmitViewModel> applicationToSubmit)
        {
            IEnumerable<HousingAppToSubmitViewModel> result = null;
            int userID = GetCurrentUserID();

            var id = _accountService.GetAccountByUsername(applicant).GordonID;
            result = _housingService.submitShiftForUser(userID, shift.EML, shift.SHIFT_END_DATETIME, shift.SUBMITTED_TO, shift.LAST_CHANGED_BY);
    
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return InternalServerError();
            }
            return Ok(result);
        }
    }
}
