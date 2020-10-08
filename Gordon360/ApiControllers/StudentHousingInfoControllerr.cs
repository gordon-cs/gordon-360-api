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
    [RoutePrefix("api/ApartmentApp")] 
    [Authorize]
    [CustomExceptionFilter]
    public class StudentHousingInfoController : ApiController
    {
        private IStudentHousingInfoService _studentHousingInfoService;

        /** Call the service that gets all student housing information
         */
        [HttpGet]
        [Route("End-to-End")] //TODO follow convention of lowercase
        public IHttpActionResult GetStudentHousingInfo()
        {
            var result = _studentHousingInfoService.Get();
            return Ok(result);
        }
    }
}
