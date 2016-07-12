using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gordon360.Services;
using Gordon360.Repositories;
using System.Linq;
using System.Net.Http;

namespace Gordon360.AuthorizationFilters
{
    /* ActivityFilter Masquerading as Authorization filter
     * Each filter Authorizes the role indicated by the filter.
     * */
    public class AuthorizeActivityLeader : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var user = actionContext.RequestContext.Principal as ClaimsPrincipal;
                var userID = user.Claims.FirstOrDefault(x => x.Type == "id");
                var unitOfWork = new UnitOfWork();
                var activityService = new ActivityService(unitOfWork);
                var activityCode = (string)actionContext.ActionArguments["id"];
                var leaders = activityService.GetLeadersForActivity(activityCode);

                var isLeader = leaders.Where(x => x.IDNumber == userID.Value && Constants.Leaders.Contains(x.Participation)).Count() > 0;
                if(isLeader)
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