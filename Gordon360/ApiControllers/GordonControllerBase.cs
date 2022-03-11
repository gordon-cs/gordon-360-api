using Gordon360.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.ApiControllers
{
    /* Set standard, opinionated controller behavior for all our controllers
     * With the ApiController Attribute, we enforce
     * - Attribute routing
     * - Automatic HTTP 400 responses if model validation fails
     * - Inference of binding source for route parameters
     * - Problem details for error status codes
     * (See https://docs.microsoft.com/en-us/aspnet/core/web-api/ for more info)
     * 
     * With the Authorize attribute, we enforce that all controller routes, 
     * unless otherwise specified via the AllowAnonymous attribute, require authorization.
     * Since public routes must be explicitly specified, we can more easily find/audit/update them,
     * simply by searching for all instances of the AllowAnonymous attribute
     * 
     * The CustomExceptionFilter is currently not implemented, but it's purpose is to enable us to throw
     * common exceptions from any route in a standard way that the UI recognizes.
     * 
     * With the Route attribute, we enforce a standard prefix for all controllers, "api/[name of controller]".
     * For example, in a class named StudentsController, every route in that controller will be prefixed with "api/students"
     * Centralizing this prefix also allows us to adjust it in the future more easily, if desired.
     */
    [ApiController]
    [Authorize]
    [CustomExceptionFilter]
    [Route("api/[controller]")]
    public class GordonControllerBase : ControllerBase
    {
    }
}
