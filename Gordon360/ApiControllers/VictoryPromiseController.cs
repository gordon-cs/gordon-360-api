using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/vpscore")]
    [CustomExceptionFilter]
    [Authorize]
    public class VictoryPromiseController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IVictoryPromiseService _victoryPromiseService;

        public VictoryPromiseController()
        {
            var _unitOfWork = new UnitOfWork();
            _victoryPromiseService = new VictoryPromiseService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }
        public VictoryPromiseController(IVictoryPromiseService victoryPromiseService)
        {
            _victoryPromiseService = victoryPromiseService;
        }

        /// <summary>
        ///  Gets current victory promise scores
        /// </summary>
        /// <returns>A VP Json</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;
<<<<<<< HEAD

=======
>>>>>>> 1617cf65c2793bfc3d7eeabe7ad7e0989b934060
            var result = _victoryPromiseService.GetVPScores(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}