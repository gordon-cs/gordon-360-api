using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationController : GordonControllerBase
    {
        private readonly RegistrationService _registrationService;
        public RegistrationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        /// <summary>
        /// Retrieves the registration window data for the currently logged-in user.
        /// Returns a 404 if registration info is not found (either missing account or date info).
        /// </summary>
        /// <returns>A RegistrationPeriodViewModel with eligibility and timing details</returns>
        [HttpGet]
        [Route("window")] // Endpoint: GET api/registration/window
        public async Task<ActionResult<RegistrationPeriodViewModel>> GetRegistrationWindow()
        {
            var username = AuthUtils.GetUsername(User);
            var result = await _registrationService.GetRegistrationWindowAsync(username);
            if (result == null)
            {
                return NotFound(new { error = "Registration period not found for user." });
            }
            return Ok(result);
        }
    }
}