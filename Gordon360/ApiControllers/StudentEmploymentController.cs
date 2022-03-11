using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gordon360.ApiControllers
{
    [Route("api/[controller]")]
    public class StudentEmploymentController : ControllerBase
    {
        //declare services we are going to use.
        private readonly IStudentEmploymentService _studentEmploymentService;

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
