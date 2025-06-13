using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;

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
        /// Get the registration window info for the current user.
        /// Returns 404 with error message if not found.
        /// </summary>
        [HttpGet]
        [Route("window")]
        public ActionResult<RegistrationPeriodViewModel> GetRegistrationWindow()
        {
            var username = AuthUtils.GetUsername(User);

            var result = _registrationService.GetRegistrationWindow(username);

            if (result == null)
            {
                // Return 404 with a JSON error message
                return NotFound(new { error = "Registration dates not found for user." });
            }

            return Ok(result);
        }
    }
}
