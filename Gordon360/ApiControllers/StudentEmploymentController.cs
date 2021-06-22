using System.Security.Claims;
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
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IStudentEmploymentService _studentEmploymentService;

        public StudentEmploymentController()
        {
            var _unitOfWork = new UnitOfWork();
            _studentEmploymentService = new StudentEmploymentService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
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
