using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Static.Names;
using System.Linq;
using System.Net.Http;

namespace Gordon360.AuthorizationFilters
{
    public class StateYourBusiness : ActionFilterAttribute
    {
        public string resource { get; set; }
        public string operation { get; set; }

        private HttpActionContext context;
        private string user_position { get; set; }
        private string user_id { get; set; }
        private bool isAuthorized = false;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var authenticatedUser = actionContext.RequestContext.Principal as ClaimsPrincipal;
            user_position = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "college_role").Value;
            user_id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;

            // Verify if the user can operate on the resource
            isAuthorized = canAccessResource(user_position, resource);
            if(!isAuthorized) 
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            // The user has access to the resource. Can the user perfom the operation on the resource?
            isAuthorized = canPerformOperation(resource, operation);
            if(!isAuthorized)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
            



        }

        private bool canAccessResource(string user_position,string resource)
        {
            switch(resource)
            {
                case Resource.MEMBERSHIP: return canAccessMembership(user_position);
                case Resource.MEMBERSHIP_REQUEST: return canAccessMembershipRequest(user_position);
                case Resource.STUDENT: return canAccessStudent(user_position);
                case Resource.SUPERVISOR: return canAccessSupervisor(user_position);
                default: return false;
            }
        }
        private bool canPerformOperation(string resource, string operation)
        {
            switch(operation)
            {
                case Operation.READ_ONE: return canReadOne(resource);
                case Operation.READ_ALL: return canReadAll(resource);
                case Operation.ADD: return canAdd(resource);
                case Operation.UPDATE: return canUpdate(resource);
                case Operation.DELETE: return canDelete(resource);
                default: return false;
            }
        }
        /*
         * Resources
         */
        private bool canAccessMembership(string user_position)
        {
            switch(user_position)
            {
                case Position.STUDENT: return true;
                case Position.FACSTAFF: return true;
                case Position.GOD: return true;
                default: return false;
            }
        }
        private bool canAccessMembershipRequest(string user_position)
        {
            switch(user_position)
            {
                case Position.STUDENT: return true;
                case Position.FACSTAFF: return true;
                case Position.GOD: return true;
                default: return false;
            }
        }
        private bool canAccessStudent(string user_position)
        {
            switch(user_position)
            {
                // We restrict student access to student resource here. A student should never need to touch this resource.
                // Q: What if i need to know stuff about MY student resource? 
                // A: You should already know info about yourself. Don't need a computer to tell you that.
                case Position.STUDENT: return false;
                case Position.FACSTAFF: return true;
                case Position.GOD: return true;
                default: return false;
            }
        }
        private bool canAccessSupervisor(string user_position)
        {
            switch (user_position)
            {
                case Position.STUDENT: return false;
                case Position.FACSTAFF: return true;
                case Position.GOD: return true;
                default: return false;
            }
        }

        /*
         * Operations
         * 
         */
         private bool canReadOne(string resource)
        {
            switch(resource)
            {
                
                case Resource.MEMBERSHIP:
                    return true;
                case Resource.MEMBERSHIP_REQUEST: 
                    // membershipRequest = mr
                    var mrService = new MembershipRequestService(new UnitOfWork());
                    var mrID = (int)context.ActionArguments["id"];
                    var mrToConsider = mrService.Get(mrID);
                    var is_mrOwner = mrToConsider.IDNumber == user_id; // User_id is an instance variable.

                    if (is_mrOwner) // If user owns the request
                        return true;

                    var activityCode = mrToConsider.ActivityCode;
                    var activityService = new ActivityService(new UnitOfWork());
                    var isLeader = activityService.GetLeadersForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isLeader) // If user is the leader of the activity that the request is sent to.
                        return true;

                    var isSupervisor = activityService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isSupervisor) // If user is a supervisor of the activity that the request is sent to
                        return true;

                    return false;
                case Resource.STUDENT:
                    return false;  // For now, I'm thinking no one (excpet GOD) should ever neeed to reed student data
                    // Enough information for a student is available in the membership resource.
                    
                case Resource.SUPERVISOR:
                    // sup = supervisor
                    var supService = new SupervisorService(new UnitOfWork());
                    var supID = (int)context.ActionArguments["id"];
                    var supToConsider = supService.Get(supID);
                    var is_supOwner = supToConsider.IDNumber == user_id; // User_id is an instance variable.
                    if (is_supOwner) // User is the supervisor
                        return true;
                    return false;
            }
        } 
        private bool canReadAll(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    return true;
                case Resource.MEMBERSHIP_REQUEST:
                    break;
                case Resource.STUDENT:
                    break;
                case Resource.SUPERVISOR:
                    break;
            }
        }
        private bool canAdd(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    break;
                case Resource.MEMBERSHIP_REQUEST:
                    break;
                case Resource.STUDENT:
                    break;
                case Resource.SUPERVISOR:
                    break;
            }
        }
        private bool canUpdate(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    break;
                case Resource.MEMBERSHIP_REQUEST:
                    break;
                case Resource.STUDENT:
                    break;
                case Resource.SUPERVISOR:
                    break;
            }
        }
        private bool canDelete(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    break;
                case Resource.MEMBERSHIP_REQUEST:
                    break;
                case Resource.STUDENT:
                    break;
                case Resource.SUPERVISOR:
                    break;
            }
        }


        /*
         * Utility Methods
         */
         private bool isOwner(, )
       
    }
}