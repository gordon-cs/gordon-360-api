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
     * Each Filter authorizes the role indicated by the filter.
     * */
     public class AuthorizeSupervisor: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var user = actionContext.RequestContext.Principal as ClaimsPrincipal;
                var userRole = user.Claims.FirstOrDefault(x => x.Type == "college_role");
                if(userRole.Value != Constants.FACSTAFF_ROLE)
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
  
                }
                else
                {
                    var userId = user.Claims.FirstOrDefault(x => x.Type == "id");
                    var activityCode = (string)actionContext.ActionArguments["id"];

                    var unitOfWork = new UnitOfWork();
                    var activityService = new ActivityService(unitOfWork);
                    
                    var supervisors = activityService.GetSupervisorsForActivity(activityCode);
                    var isSupervisor = supervisors.Where(x => x.IDNumber == userId.Value).Count() > 0;
                    if(!isSupervisor)
                    {
                        actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        base.OnActionExecuting(actionContext);
                    }
                }

            }
        }
    }
}