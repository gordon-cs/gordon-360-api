using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/academicCheckIn")]
    [CustomExceptionFilter]
    [Authorize]
    public class AcademicCheckInController : ApiController
    {
        private IAccountService _accountService;

        private IacademicCheckInService _academicCheckInService;

        public academicCheckInController()
        {
            var _unitOfWork = new UnitOfWork();
            _academicCheckInService = new AcademicCheckInService();
            _accountService = new AccountService(_unitOfWork);
        }

        public academicCheckInController(IAcademicCheckInService academicCheckInService)
        {
            _academicCheckInService = academicCheckInService;
        }

        /// <summary>
        /// Enum representing two possible academicCheckIn statuses.
        /// ONCAMPUS - Student is on campus
        /// YELLOW - Symptomatic or cautionary hold
        /// RED - Quarantine/Isolation
        /// </summary>
        public enum academicCheckInStatus
        {
            GREEN, YELLOW, RED
        }

        /// <summary>
        ///  Gets academicCheckIn status of current user
        /// </summary>
        /// <returns>A academicCheckInViewModel representing the most recent status of the user</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _academicCheckInService.GetStatus(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets question for academicCheckIn check from the back end
        /// </summary>
        /// <returns>A academicCheckInQuestionViewModel</returns>
        [HttpGet]
        [Route("question")]
        public IHttpActionResult GetQuestion()
        {
            var result = _academicCheckInService.GetQuestion();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Stores the user's academicCheckIn status
        /// </summary>
        /// <param name="status">The current status of the user to post, of type academicCheckInStatusColor</param>
        /// <returns>The status that was stored in the database</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] academicCheckInStatusColor status)
        {

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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _academicCheckInService.PostStatus(status, id);

            if (result == null)
            {
                return NotFound();
            }


            return Created("Recorded answer :", result);

        }
    }
}
