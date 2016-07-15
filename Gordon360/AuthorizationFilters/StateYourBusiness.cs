using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Static.Names;
using System.Linq;
using System.Net.Http;
using Gordon360.Models;
using System.Diagnostics;

namespace Gordon360.AuthorizationFilters
{
    /* Authorization Filter.
     * It is actually an action filter masquerading as an authorization filter. This is because I need access to the 
     * parameters passed to the controller. Authorizatoin Filters don't have that access. Action Filters do.
     * 
     * Because of the nature of how we authorize people, this code might seem very odd, so I'll try to explain. 
     * Proceed at your own risk. If you can understand this code, you can understand the whole project. 
     * 
     * 1st Observation: You can't authorize access to a resource that isn't owned by someone. Resources like Sessions, Participations,
     * and Activity Definitions are accessbile by anyone.
     * 2nd Observation: To Authorize someone to perform an action on a resource, you need to know the following:
     * 1. Who is to be authorized? 2.What resource are they trying to access? 3. What operation are they trying to make on the resource?
     * The goes through those three points sequentially and evaluates through a series of switch statements if the current user
     * is authorized.
     */ 
    public class StateYourBusiness : ActionFilterAttribute
    {
        // Rousource to be accessed: Will get as parameters to the attribute
        public string resource { get; set; }
        // Operation to be performed: Will get as parameters to the attribute
        public string operation { get; set; }

        private HttpActionContext context;

        // User position at the college and their id.
        private string user_position { get; set; }
        private string user_id { get; set; }

        private bool isAuthorized = false;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            context = actionContext;
            // Step 1: Who is to be authorized
            var authenticatedUser = actionContext.RequestContext.Principal as ClaimsPrincipal;
            user_position = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "college_role").Value;
            user_id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            // If user is god, skip verification steps and return.
            if (user_position == Position.GOD)
            {
                base.OnActionExecuting(actionContext);
                return;
            }
            // Step 2: Verify if the user can operate on the resource
            isAuthorized = canAccessResource(user_position, resource);
            if(!isAuthorized) 
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            // The user has access to the resource. Step 3:Can the user perfom the operation on the resource?
            isAuthorized = canPerformOperation(resource, operation);
            if(!isAuthorized)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            base.OnActionExecuting(actionContext);
        }

        private bool canAccessResource(string user_position,string resource)
        {
            switch(resource)
            {
                // All three options below result in the same thing.
                case Resource.MEMBERSHIP_BY_ACTIVITY:
                case Resource.MEMBERSHIP_BY_STUDENT:
                case Resource.MEMBERSHIP:
                    return canAccessMembership(user_position);
                // All three options below resutl in the same thing.
                case Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY:
                case Resource.MEMBERSHIP_REQUEST_BY_STUDENT:
                case Resource.MEMBERSHIP_REQUEST:
                    return canAccessMembershipRequest(user_position);
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
                case Operation.READ_PARTIAL: return canReadPartial(resource);
                case Operation.ADD: return canAdd(resource);
                case Operation.DENY_ALLOW: return canDenyAllow(resource);
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
                case Position.STUDENT: return true;
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

         // This operation is specifically for authorizing deny and allow operations on membership requests. These two operations don't
         // Fit in nicely with the REST specificatino which is why there is a seperate case for them.
         private bool canDenyAllow(string resource)
        {
            switch(resource)
            {
                
                case Resource.MEMBERSHIP_REQUEST:
                    {
                        var mrID = (int)context.ActionArguments["id"];
                        // Get the veiw model from the repository
                        var mrService = new MembershipRequestService(new UnitOfWork());
                        var mrToConsider = mrService.Get(mrID);
                        // Populate the membershipRequest manually. Omit fields I don't need.
                        var activityCode = mrToConsider.ActivityCode;
                        var membershipService = new MembershipService(new UnitOfWork());
                        var is_activityLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_activityLeader) // If user is the leader of the activity that the request is sent to.
                            return true;
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var is_mrSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_mrSupervisor) // If user is a supervisor of the activity that the request is sent to
                            return true;

                        return false;
                    }
                default: return false;
                    
            }
        }

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
                    var membershipService = new MembershipService(new UnitOfWork());
                    var isLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isLeader) // If user is the leader of the activity that the request is sent to.
                        return true;

                    var supervisorService = new SupervisorService(new UnitOfWork());
                    var isSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isSupervisor) // If user is a supervisor of the activity that the request is sent to
                        return true;

                    return false;
                case Resource.STUDENT:
                    return true; // To add a membership for a student, you need to have the students identifier. So at least, they should be able
                    // To read a record with a specific identifier
                case Resource.SUPERVISOR:
                    return true;
                default: return false;
                    
            }
        }
        // For reads that access a group of resources filterd in a specific way 
        private bool canReadPartial(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP_BY_ACTIVITY:
                    return true;
                case Resource.MEMBERSHIP_BY_STUDENT:
                    return true;
                case Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY:
                    {
                        // An activity leader should be able to see the membership requests that belong to the activity he is leading.
                        var membershipService = new MembershipService(new UnitOfWork());
                        var activityCode = (string)context.ActionArguments["id"];
                        var activityLeaders = membershipService.GetLeaderMembershipsForActivity(activityCode);
                        var is_activityLeader = activityLeaders.Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_activityLeader)
                            return true;
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var activitySupervisors = supervisorService.GetSupervisorsForActivity(activityCode);
                        var is_activitySupervisor = activitySupervisors.Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_activitySupervisor)
                            return true;
                        return false;
                    }
                // No one except god should be able to see all the membership requests a student has
                case Resource.MEMBERSHIP_REQUEST_BY_STUDENT:
                    return false;
                case Resource.SUPERVISOR_BY_ACTIVITY:
                    return true;
                default: return false;
            }
        }
        private bool canReadAll(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    return true;
                case Resource.MEMBERSHIP_REQUEST:
                    return false;
                case Resource.STUDENT:
                    return false; // See reasons for this in CanReadOne(). No one (except for god) should be able to access student records through
                    // our API.
                case Resource.SUPERVISOR:
                    return false;
                default: return false;
            }
        }
        private bool canAdd(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    var membershipToConsider = (Membership)context.ActionArguments["membership"];
                    // A membership can always be added if it is of type "GUEST"
                    var isFollower = (membershipToConsider.PART_LVL == Activity_Roles.GUEST) && (user_id == membershipToConsider.ID_NUM);
                    if (isFollower)
                        return true;

                    var activityCode = membershipToConsider.ACT_CDE;
                    var membershipService = new MembershipService(new UnitOfWork());
                    var isLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isLeader) // If user is the leader of the activity to which the membership is added
                        return true;
                    var supervisorService = new SupervisorService(new UnitOfWork());
                    var isSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                    if (isSupervisor) // If user is a supervisor of the activity to which the memberhsip is added
                        return true;

                    return false;
                case Resource.MEMBERSHIP_REQUEST:
                    return true; // Anyone should be able to spawn a membership request.
                case Resource.STUDENT:
                    return false; // No one should be able to add students through this API
                case Resource.SUPERVISOR:
                    return false; // Only god can add Supervisors through this API
                default: return false;
            }
        }
        private bool canUpdate(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    {
                        var membershipToConsider = (Membership)context.ActionArguments["membership"];
                        var is_membershipOwner = membershipToConsider.ID_NUM == user_id;
                        if (is_membershipOwner)
                            return true;

                        var activityCode = membershipToConsider.ACT_CDE;
                        var membershipService = new MembershipService(new UnitOfWork());
                        var is_membershipLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_membershipLeader)
                            return true; // Activity Leaders can update memberships of people in their activity.
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var isSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (isSupervisor)
                            return true; // supervisors for an activity can update memberships within that activity

                        return false;
                    }
                    
                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // membershipRequest = mr
                        var mrToConsider = (Request)context.ActionArguments["membershipRequest"];
                        var activityCode = mrToConsider.ACT_CDE;
                        var membershipService = new MembershipService(new UnitOfWork());
                        var is_activityLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_activityLeader) // If user is the leader of the activity that the request is sent to.
                            return true;
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var is_mrSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_mrSupervisor) // If user is a supervisor of the activity that the request is sent to
                            return true;

                        return false;
                    }
                case Resource.STUDENT:
                    return false; // No one should be able to update a student through this API
                case Resource.SUPERVISOR:
                    // sup = supervisor
                    var supService = new SupervisorService(new UnitOfWork());
                    var supToConsider = (SUPERVISOR)context.ActionArguments["supervisor"];
                    var is_supOwner = supToConsider.ID_NUM == user_id; // User_id is an instance variable.
                    if (is_supOwner) // User is the supervisor
                        return true;

                    return false;
                default: return false;
            }
        }
        private bool canDelete(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    {
                        var membershipService = new MembershipService(new UnitOfWork());
                        var membershipID = (int)context.ActionArguments["id"];
                        var membershipToConsider = membershipService.Get(membershipID);
                        var is_membershipOwner = membershipToConsider.IDNumber == user_id;
                        if (is_membershipOwner)
                            return true;

                        var activityCode = membershipToConsider.IDNumber;
                        var is_membershipLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_membershipLeader)
                            return true;
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var isSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (isSupervisor)
                            return true;

                        return false;
                    }
                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // membershipRequest = mr
                        var mrService = new MembershipRequestService(new UnitOfWork());
                        var mrID = (int)context.ActionArguments["id"];
                        var mrToConsider = mrService.Get(mrID);
                        var is_mrOwner = mrToConsider.IDNumber == user_id;
                        if (is_mrOwner)
                            return true;
                        // Use of mr here is to differentiate from variables declared in other case statements.
                        var activityCode = mrToConsider.IDNumber;
                        var membershipService = new MembershipService(new UnitOfWork());
                        var is_mrLeader = membershipService.GetLeaderMembershipsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_mrLeader)
                            return true;
                        var supervisorService = new SupervisorService(new UnitOfWork());
                        var is_mrSupervisor = supervisorService.GetSupervisorsForActivity(activityCode).Where(x => x.IDNumber == user_id).Count() > 0;
                        if (is_mrSupervisor)
                            return true;

                        return false;
                    }
                case Resource.STUDENT:
                    return false; // No one should be able to delete a student through our API
                case Resource.SUPERVISOR:
                    return false;
                default: return false;
            }
        }

       
    }
}