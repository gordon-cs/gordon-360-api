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
        [Route("save")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.HOUSING)]
        public IHttpActionResult SaveApplication(int id)
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

            _housingService.SaveApplication(id);

            return Ok();

        }
    }
}
