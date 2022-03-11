using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gordon360.ApiControllers
{
    [Route("api/[controller]")]
    public class WellnessController : GordonControllerBase
    {
        private readonly IWellnessService _wellnessService;

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
