using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gordon360.Services;
using System.Linq;
using System.Net.Http;

namespace Gordon360.AuthorizationFilters
{
    public class AuthorizeActivityLeader : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var user = actionContext.RequestContext.Principal as ClaimsPrincipal;

                var activityCode = (string)actionContext.ActionArguments["id"];
                if(user.HasClaim(x => x.Type == activityCode && Constants.Leaders.Contains(x.Value)))
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }                
            }
        }
    }
}