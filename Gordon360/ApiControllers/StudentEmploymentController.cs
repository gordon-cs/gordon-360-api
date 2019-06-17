using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/studentemployment")]
    [CustomExceptionFilter]
    [Authorize]
    public class StudentEmploymentController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IStudentEmploymentService _studentEmploymentService;

        public StudentEmploymentController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
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
        ///  Gets current victory promise scores
        /// </summary>
        /// <returns>A VP object object</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;
            var result = _studentEmploymentService.GetEmployment(id);
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine("Result is null");
                return NotFound();
            }
            return Ok(result);
        }
    }
}