using System.Linq;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Models;
using Gordon360.Models.CCT;
using Gordon360.Database.CCT;

namespace Gordon360.Controllers.Api
{
    [Route("api/wellness")]
    [CustomExceptionFilter]
    [Authorize]
    public class WellnessController : ControllerBase
    {
        private readonly IWellnessService _wellnessService;

        public WellnessController(CCTContext context)
        {
            _wellnessService = new WellnessService(context);
        }

        public WellnessController(IWellnessService wellnessService)
        {
            _wellnessService = wellnessService;
        }

        /// <summary>
        /// Enum representing three possible wellness statuses.
        /// GREEN - Healthy, no known contact/symptoms
        /// YELLOW - Symptomatic or cautionary hold
        /// RED - Quarantine/Isolation
        /// </summary>
        public enum WellnessStatusColor
        {
            GREEN, YELLOW, RED
        }

        /// <summary>
        ///  Gets wellness status of current user
        /// </summary>
        /// <returns>A WellnessViewModel representing the most recent status of the user</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<WellnessViewModel> Get()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _wellnessService.GetStatus(authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets question for wellness check from the back end
        /// </summary>
        /// <returns>A WellnessQuestionViewModel</returns>
        [HttpGet]
        [Route("question")]
        public ActionResult<WellnessQuestionViewModel> GetQuestion()
        {
            var result = _wellnessService.GetQuestion();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Stores the user's wellness status
        ///  @TODO: Return view model rather than Health_Status model directly
        /// </summary>
        /// <param name="status">The current status of the user to post, of type WellnessStatusColor</param>
        /// <returns>The status that was stored in the database</returns>
        [HttpPost]
        [Route("")]
        public ActionResult<Health_Status> Post([FromBody] WellnessStatusColor status)
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

            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _wellnessService.PostStatus(status, authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Created("Recorded answer :", result);
        }
    }
}
