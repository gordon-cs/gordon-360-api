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

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/housing")] 
    [Authorize]
    [CustomExceptionFilter]
    public class housingController : ApiController
    {
        private IHousingService _housingService;

        /** Call the service that gets all student housing information
         */
        [HttpGet]
        [Route("end-to-end")]
        public IHttpActionResult GetStudentInfo()
        {
            var result = _studentHousingInfoService.Get();
            return Ok(result);
        }
    }
}
