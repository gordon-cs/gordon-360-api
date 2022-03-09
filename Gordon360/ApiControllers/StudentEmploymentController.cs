using System.Security.Claims;
using Gordon360.Database.CCT;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers.Api
{
    [Route("api/studentemployment")]
    [CustomExceptionFilter]
    [Authorize]
    public class StudentEmploymentController : ControllerBase
    {
        //declare services we are going to use.
        private readonly IStudentEmploymentService _studentEmploymentService;

        public StudentEmploymentController(CCTContext context)
        {
            _studentEmploymentService = new StudentEmploymentService(context);
        }
        public StudentEmploymentController(IStudentEmploymentService studentEmploymentService)
        {
            _studentEmploymentService = studentEmploymentService;
        }

        /// <summary>
        ///  Gets student employment information about the user
        /// </summary>
        /// <returns>A Student Employment Json</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<StudentEmploymentViewModel> Get()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _studentEmploymentService.GetEmployment(authenticatedUserIdString);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
