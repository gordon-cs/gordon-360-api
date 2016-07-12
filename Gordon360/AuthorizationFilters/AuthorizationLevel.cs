using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gordon360.Services;
using Gordon360.Repositories;
using System.Linq;
using System.Net.Http;

namespace Gordon360.AuthorizationFilters
{
    public class AuthorizationLevel : ActionFilterAttribute
    {
        public string authorizationLevel { get; set; }
        private HttpActionContext _context { get; set; }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Make the context available to other members of the class.
            _context = actionContext;

            var isAuthorized = isAuthorizedForOne();
            if(isAuthorized)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
            
        }

        /* 
         * The Authorization Hierarchy. Member < Activity Leader < Supervisor < God
         * Any member of the hierarchy also has authorization to perform actions that belong to the member on the left
         * e.g. An activity leader can perform actions that a member can do. A supervisor can perform actions that
         * an activity Leader and a member can do etc...
         */

        public bool isAuthorizedForOne()
        {
            if(authorizationLevel == Constants.MEMBER)
            {
                return isAuthorizedMember() || isAuthorizedActivityLeader() || isAuthorizedSupervisor() || isAuthorizedGod();
            }
            else if(authorizationLevel == Constants.ACTIVITY_LEADER_LEVEL)
            {
                return isAuthorizedActivityLeader() || isAuthorizedSupervisor() || isAuthorizedGod();
            }
            else if(authorizationLevel == Constants.SUPERVISOR_LEVEL)
            {
                return isAuthorizedSupervisor() || isAuthorizedGod();
            }
            else if(authorizationLevel == Constants.GOD_LEVEL)
            {
                return isAuthorizedGod();
            }
            else
            {
                return false;
            }
        }
        // Are you a member of this activity?
        private bool isAuthorizedMember()
        {
            var user = _context.RequestContext.Principal as ClaimsPrincipal;
            var userId = user.Claims.FirstOrDefault(x => x.Type == "id");

            // Get Members
            var activityService = new ActivityService(new UnitOfWork());
            var activityCode = (string)_context.ActionArguments["id"];
            var members = activityService.GetMembershipsForActivity(activityCode);

            // Are you part of the members?
            var isMember = members.Where(x => x.IDNumber == userId.Value).Count() > 0;
            if(isMember)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Are you an activity Leader?
        private bool isAuthorizedActivityLeader()
        {
            // Get User claims
            var user = _context.RequestContext.Principal as ClaimsPrincipal;
            var userID = user.Claims.FirstOrDefault(x => x.Type == "id");

            // Get Leaders for this activity
            var activityService = new ActivityService(new UnitOfWork());
            var activityCode = (string)_context.ActionArguments["id"];
            var leaders = activityService.GetLeadersForActivity(activityCode);
            // Find out if the user is one of the leaders
            var isLeader = leaders.Where(x => x.IDNumber == userID.Value && Constants.Leaders.Contains(x.Participation)).Count() > 0;
            if (isLeader)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Are you an Activity Supervisor?
        private bool isAuthorizedSupervisor()
        {
            // Get the user claims
            var user = _context.RequestContext.Principal as ClaimsPrincipal;
            var userRole = user.Claims.FirstOrDefault(x => x.Type == "college_role");
            // Weed out any non faculty/staff
            if (userRole.Value != Constants.FACSTAFF_ROLE)
            {
                return false;
            }
            else
            {
                var userId = user.Claims.FirstOrDefault(x => x.Type == "id");
                
                // Get the supervisors for this activity
                var activityService = new ActivityService(new UnitOfWork());
                var activityCode = (string)_context.ActionArguments["id"];
                var supervisors = activityService.GetSupervisorsForActivity(activityCode);
                // Find out if the user is one of the supervisors
                var isSupervisor = supervisors.Where(x => x.IDNumber == userId.Value).Count() > 0;
                if (isSupervisor)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Are you a god?
        private bool isAuthorizedGod()
        {
            return false;
        }
    }

}