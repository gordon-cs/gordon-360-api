using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Authorization;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class WellnessController : GordonControllerBase
    {
        private readonly IWellnessService _wellnessService;

        public WellnessController(CCTContext context)
        {
            _wellnessService = new WellnessService(context);
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
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            var result = _wellnessService.GetStatus(authenticatedUserUsername);

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
        /// </summary>
        /// <param name="status">The current status of the user to post, of type WellnessStatusColor</param>
        /// <returns>The status that was stored in the database</returns>
        [HttpPost]
        [Route("")]
        public ActionResult<WellnessViewModel> Post([FromBody] WellnessStatusColor status)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            var result = _wellnessService.PostStatus(status, authenticatedUserUsername);

            if (result == null)
            {
                return NotFound();
            }

            return Created("Recorded answer :", result);
        }
    }
}
