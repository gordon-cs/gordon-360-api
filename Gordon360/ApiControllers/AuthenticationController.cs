using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gordon360.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public  AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            var result = await _authenticationService.AuthenticateAsync(username, password);
            return Ok(result);
        }
    }
}
