using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/wellness")]
    [CustomExceptionFilter]
    [Authorize]
    public class WellnessController : ApiController
    {
        private IAccountService _accountService;

        private IWellnessService _wellnessService;

        public WellnessController()
        {
            var _unitOfWork = new UnitOfWork();
            _wellnessService = new WellnessService();
            _accountService = new AccountService(_unitOfWork);
        }

        public WellnessController(IWellnessService wellnessService)
        {
            _wellnessService = wellnessService;
        }

        public enum WellnessStatusColor
        {
            GREEN, YELLOW, RED
        }

        /// <summary>
        ///  DEPRECATED - Please use new route /status
        ///  Gets current wellness status of student
        /// </summary>
        /// <returns>Json WellnessViewModel</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult DEPRECATED_Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _wellnessService.DEPRECATED_GetStatus(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets current wellness status of student
        /// </summary>
        /// <returns>Json WellnessStatusViewModel</returns>
        [HttpGet]
        [Route("status")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _wellnessService.GetStatus(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets question for wellness check from the back end
        /// </summary>
        /// <returns> json WellnessQuestionViewModel</returns>
        [HttpGet]
        [Route("status/question")]
        public IHttpActionResult GetQuestion()
        {
            var result = _wellnessService.GetQuestion();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets question for wellness check from the back end
        /// </summary>
        /// <returns> json WellnessQuestionViewModel</returns>
        [HttpGet]
        [Route("question")]
        public IHttpActionResult DEPRECATED_GetQuestion()
        {
            var result = _wellnessService.DEPRECATED_GetQuestion();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// DEPRECATED - Please use new route /status
        ///  Stores the user's wellness check answer, with a timestamp.
        ///  If answer boolean is true: student is feeling symptomatic(feeling sick).
        ///  If answer boolean is false: student is not feeling symptomatic(feeling fine).
        /// </summary>
        /// <returns>Ok if message was recorded</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult DEPRECATED_PostAnswer([FromBody] bool answer)
        {

            if (!ModelState.IsValid || answer == null)
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

            var result = _wellnessService.DEPRECATED_PostStatus(answer, id);

            if (result == null)
            {
                return NotFound();
            }


            return Created("Recorded answer :", answer);

        }

        [HttpPost]
        [Route("status")]
        public IHttpActionResult PostStatus([FromBody] WellnessStatusColor status)
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

            var result = _wellnessService.PostStatus(status.ToString(), id);

            return Created("Recorded answer :", result);
        }
    }
}