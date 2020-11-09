using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models.ViewModels;

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
        ///  Gets current wellness status of student
        /// </summary>
        /// <returns>Json WellnessViewModel</returns>
        [HttpGet]
        [Route("")]
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
        [Route("question")]
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
        ///  Stores the user's wellness check answer, with a timestamp.
        ///  If answer boolean is true: student is feeling symptomatic(feeling sick).
        ///  If answer boolean is false: student is not feeling symptomatic(feeling fine).
        /// </summary>
        /// <param name="answer"> The answer to be posted. true = Symptoms, false = No Symptoms</param>
        /// <returns> Answer that was recorded successfully </returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostAnswer([FromBody] bool answer)
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

            var result = _wellnessService.PostStatus(answer, id);

            return Created("Recorded answer :", result);

        }

        [HttpGet]
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;

            var result = _wellnessService.GetStatusOrOverride(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
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