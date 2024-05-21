using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Services;
using Gordon360.Services.RecIM;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;
using Gordon360.Extensions.System;
using static Gordon360.Services.MembershipService;
using System.Linq.Expressions;
using Gordon360.Enums;
using Microsoft.Extensions.Configuration;


namespace Gordon360.Authorization
{
    /* Authorization Filter.
     * It is actually an action filter masquerading as an authorization filter. This is because I need access to the 
     * parameters passed to the controller. Authorization Filters don't have that access. Action Filters do.
     * 
     * Because of the nature of how we authorize people, this code might seem very odd, so I'll try to explain. 
     * Proceed at your own risk. If you can understand this code, you can understand the whole project. 
     * 
     * 1st Observation: You can't authorize access to a resource that isn't owned by someone. Resources like Sessions, Participations,
     * and Activity Definitions are accessibile by anyone.
     * 2nd Observation: To Authorize someone to perform an action on a resource, you need to know the following:
     * 1. Who is to be authorized? 2.What resource are they trying to access? 3. What operation are they trying to make on the resource?
     * This "algorithm" uses those three points and decides through a series of switch statements if the current user
     * is authorized.
     */
    public class StateYourBusiness : ActionFilterAttribute
    {
        // Resource to be accessed: Will get as parameters to the attribute
        public string resource { get; set; }
        // Operation to be performed: Will get as parameters to the attribute
        public string operation { get; set; }

        private ActionExecutingContext context;
        private CCTContext _CCTContext;
        private MyGordonContext _MyGordonContext;
        private IAccountService _accountService;
        private IMembershipService _membershipService;
        private IMembershipRequestService _membershipRequestService;
        private INewsService _newsService;

        private IConfiguration _config;

        //RecIM services
        private IParticipantService _recimParticipantService;
        private Services.RecIM.IActivityService _recimActivityService;
        private ISeriesService _recimSeriesService;
        private ITeamService _recimTeamService;
        private IMatchService _recimMatchService;

        // User position at the college and their id.
        private IEnumerable<AuthGroup> user_groups { get; set; }
        private string user_id { get; set; }
        private string user_name { get; set; }

        public async override Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            context = actionContext;
            // Step 1: Who is to be authorized
            var authenticatedUser = actionContext.HttpContext.User;

            _accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();
            _membershipService = context.HttpContext.RequestServices.GetRequiredService<IMembershipService>();
            _membershipRequestService = context.HttpContext.RequestServices.GetRequiredService<IMembershipRequestService>();
            _newsService = context.HttpContext.RequestServices.GetRequiredService<INewsService>();
            _CCTContext = context.HttpContext.RequestServices.GetService<CCTContext>();
            _MyGordonContext = context.HttpContext.RequestServices.GetService<MyGordonContext>();

            // set RecIM services
            _recimParticipantService = context.HttpContext.RequestServices.GetRequiredService<IParticipantService>();
            _recimMatchService = context.HttpContext.RequestServices.GetRequiredService<IMatchService>();
            _recimTeamService = context.HttpContext.RequestServices.GetRequiredService<ITeamService>();
            _recimSeriesService = context.HttpContext.RequestServices.GetRequiredService<ISeriesService>();
            _recimActivityService = context.HttpContext.RequestServices.GetRequiredService<Services.RecIM.IActivityService>();

            user_name = AuthUtils.GetUsername(authenticatedUser);
            user_groups = AuthUtils.GetGroups(authenticatedUser);
            user_id = _accountService.GetAccountByUsername(user_name).GordonID;

            if (user_groups.Contains(AuthGroup.SiteAdmin))
            {
                await next();
                return;
            }

            bool isAuthorized = await CanPerformOperationAsync(resource, operation);
            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException("Authorization has been denied for this request.");
            }

            await next();
        }


        private async Task<bool> CanPerformOperationAsync(string resource, string operation)
            => operation switch
            {
                Operation.READ_ONE => await CanReadOneAsync(resource),
                Operation.READ_ALL => CanReadAll(resource),
                Operation.READ_PARTIAL => await CanReadPartialAsync(resource),
                Operation.ADD => await CanAddAsync(resource),
                Operation.UPDATE => await CanUpdateAsync(resource),
                Operation.DELETE => await CanDeleteAsync(resource),
                Operation.READ_PUBLIC => CanReadPublic(resource),
                _ => false,
            };

        /*
         * Operations
         */
        private async Task<bool> CanReadOneAsync(string resource)
        {
            // User is admin
            if (user_groups.Contains(AuthGroup.SiteAdmin))
                return true;

            switch (resource)
            {
                case Resource.PROFILE:
                    return true;
                case Resource.EMERGENCY_CONTACT:
                    {
                        if (user_groups.Contains(AuthGroup.Police))
                            return true;

                        return context.ActionArguments["username"] is string username && username.EqualsIgnoreCase(user_name);
                    }
                case Resource.MEMBERSHIP:
                    return true;
                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // membershipRequest = mr
                        if (context.ActionArguments["id"] is int mrID)
                        {
                            var mrToConsider = _membershipRequestService.Get(mrID);
                            var is_mrOwner = mrToConsider.Username.EqualsIgnoreCase(user_name); // User_id is an instance variable.

                            if (is_mrOwner) // If user owns the request
                                return true;

                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: mrToConsider.ActivityCode,
                                    username: user_name,
                                    sessionCode: mrToConsider.SessionCode,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            if (isGroupAdmin) // If user is a group admin of the activity that the request is sent to
                                return true;
                        }

                        return false;
                    }
                case Resource.STUDENT:
                    // To add a membership for a student, you need to have the students identifier.
                    // NOTE: I don't believe the 'student' resource is currently being used in API
                    {
                        return true;
                    }
                case Resource.ADVISOR:
                    return true;
                case Resource.ACCOUNT:
                    {
                        // Membership group admins can access ID of members using their email
                        // NOTE: In the future, probably only email addresses should be stored 
                        // in memberships, since we would rather not give students access to
                        // other students' account information
                        var isGroupAdmin = _membershipService.IsGroupAdmin(user_name);
                        if (isGroupAdmin) // If user is a group admin of the activity that the request is sent to
                            return true;

                        // faculty and police can access student account information
                        if (user_groups.Contains(AuthGroup.FacStaff)
                            || user_groups.Contains(AuthGroup.Police))
                            return true;

                        return false;
                    }
                case Resource.HOUSING:
                    {
                        // The members of the apartment application can only read their application
                        var sess_cde = Helpers.GetCurrentSession(_CCTContext);
                        HousingService housingService = new HousingService(_CCTContext);
                        int? applicationID = housingService.GetApplicationID(user_name, sess_cde);
                        if (context.ActionArguments["applicationID"] is int requestedApplicationID && applicationID is not null)
                        {
                            return requestedApplicationID == applicationID;
                        }
                        return false;
                    }
                case Resource.NEWS:
                    return true;
                default: return false;

            }
        }
        // For reads that access a group of resources filterd in a specific way 
        private async Task<bool> CanReadPartialAsync(string resource)
        {
            // User is admin
            if (user_groups.Contains(AuthGroup.SiteAdmin))
                return true;

            switch (resource)
            {
                case Resource.MEMBERSHIP_BY_ACTIVITY:
                    {
                        // Only people that are part of the activity should be able to see members
                        if (context.ActionArguments["activityCode"] is string activityCode)
                        {
                            var activityMembers = _membershipService.GetMemberships(activityCode: activityCode, username: user_name);
                            var is_personAMember = activityMembers.Any(x => x.Participation != Participation.Guest.GetCode());
                            return is_personAMember;
                        }
                        return false;
                    }

                case Resource.MEMBERSHIP:
                    {
                        // Everyone can read a specific user's memberships
                        // TODO: restrict if user is private?
                        if (context.ActionArguments.TryGetValue("username", out object? username_object) && username_object is string username)
                        {
                            return true;
                        }

                        // Only members can read a specific activity's memberships
                        if (context.ActionArguments.TryGetValue("involvementCode", out object? involvementCode_object)  && involvementCode_object is string involvementCode)
                        {
                            if (context.ActionArguments.TryGetValue("sessionCode", out object? sessionCode_object) && sessionCode_object is string sessionCode)
                            {
                                var activityMembers = _membershipService.GetMemberships(activityCode: involvementCode, username: user_name, sessionCode: sessionCode);
                                var is_personAMember = activityMembers.Any(x => x.Participation != "GUEST");
                                return is_personAMember;

                            }
                            else
                            {
                                var activityMembers = _membershipService.GetMemberships(activityCode: involvementCode, username: user_name, sessionCode: "*");
                                var is_personAMember = activityMembers.Any(x => x.Participation != "GUEST");
                                return is_personAMember;
                            }
                        }


                        return false;
                    }

                case Resource.EVENTS_BY_STUDENT_ID:
                    {
                        // Only the person itself or an admin can see someone's chapel attendance
                        if (context.ActionArguments["username"] is string username_requested)
                        {
                            return username_requested.EqualsIgnoreCase(user_name);
                        }
                        return false;
                    }


                case Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY:
                    {
                        // An activity leader should be able to see the membership requests that belong to the activity s/he is leading.
                        if (context.ActionArguments["activityCode"] is string activityCode)
                        {
                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            return isGroupAdmin; // If user is a group admin of the activity that the request is sent to
                        }
                        return false;
                    }

                case Resource.EMAILS_BY_ACTIVITY:
                    {
                        var publicParticipantTypes = new List<string>
                                {
                                    Participation.GroupAdmin.GetCode(),
                                    Participation.Advisor.GetCode()
                                };

                        // Anyone can view group-admin and advisor emails
                        // TODO: Remove once Obsolete EmailsController routes are gone
                        if (context.ActionArguments.TryGetValue("participationType", out var participationType)
                            && participationType is string participation
                            && participation.In(publicParticipantTypes.ToArray())
                            )
                        {
                            return true;
                        }

                        // Anyone can view group-admin and advisor emails
                        if (context.ActionArguments.TryGetValue("participationTypes", out var p)
                            && p is List<string> participationTypes
                            && participationTypes.All(pt => pt.In(publicParticipantTypes.ToArray()))
                            )
                        {
                            return true;
                        }

                        var leaderTypes = new List<string>
                                {
                                    Participation.GroupAdmin.GetCode(),
                                    Participation.Leader.GetCode(),
                                    Participation.Advisor.GetCode()
                                };

                        // Only leaders, advisors, and group admins
                        if (context.ActionArguments["activityCode"] is string activityCode)
                        {
                            string? sessionCode = context.ActionArguments.TryGetValue("sessionCode", out var sessionCodeObject) ? sessionCodeObject as string : null;
                            return _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    sessionCode: sessionCode,
                                    participationTypes: leaderTypes)
                                .Any();
                        }

                        return false;
                    }
                case Resource.NEWS:
                    {
                        return true;
                    }
                default: return false;
            }
        }
        private bool CanReadAll(string resource)
        {
            switch (resource)
            {
                case Resource.MEMBERSHIP:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false;
                case Resource.ChapelEvent:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false;
                case Resource.EVENTS_BY_STUDENT_ID:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false;

                case Resource.MEMBERSHIP_REQUEST:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false;
                case Resource.STUDENT:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false; // See reasons for this in CanReadOne(). No one (except for super admin) should be able to access student records through
                                      // our API.
                case Resource.ADVISOR:
                    // User is authorized to view academic info
                    if (user_groups.Contains(AuthGroup.AcademicInfoView))
                        return true;
                    // User looks his or her own profile
                    else if (context.ActionArguments["username"] is string username && username.EqualsIgnoreCase(user_name))
                        return true;
                    else
                        return false;
                case Resource.GROUP_ADMIN:
                    // User is site-wide admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false;
                case Resource.ACCOUNT:
                    return false;
                case Resource.ADMIN:
                    return false;
                case Resource.HOUSING:
                    {
                        // Only the housing admin and super admin can read all of the received applications.
                        // Super admin has unrestricted access by default, so no need to check.
                        if (user_groups.Contains(AuthGroup.HousingAdmin))
                        {
                            return true;
                        }
                        return false;
                    }
                case Resource.NEWS:
                    return user_groups.Contains(AuthGroup.NewsAdmin);
                case Resource.RECIM:
                    return _recimParticipantService.IsAdmin(user_name);
                default: return false;
            }
        }

        private bool CanReadPublic(string resource)
        {
            switch (resource)
            {
                case Resource.SLIDER:
                    return true;
                case Resource.NEWS:
                    return false;
                default: return false;

            }
        }
        private async Task<bool> CanAddAsync(string resource)
        {
            switch (resource)
            {
                case Resource.SHIFT:
                    {
                        if (user_groups.Contains(AuthGroup.Student))
                            return true;
                        return false;
                    }
                case Resource.CLIFTON_STRENGTHS:
                    {
                        return user_groups.Contains(AuthGroup.SiteAdmin);
                    }

                case Resource.MEMBERSHIP:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;

                        if (context.ActionArguments["membershipUpload"] is MembershipUploadViewModel membershipToConsider)
                        {

                            // A membership can always be added if it is of type "GUEST"
                            var isFollower = membershipToConsider.Participation == Activity_Roles.GUEST
                                && membershipToConsider.Username.EqualsIgnoreCase(user_name);
                            if (isFollower)
                                return true;

                            var activityCode = membershipToConsider.Activity;
                            var sessionCode = membershipToConsider.Session;
                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    sessionCode: sessionCode,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            // If user is the advisor of the activity to which the request is sent.
                            if (isGroupAdmin)
                                return true;
                        }
                        return false;
                    }

                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;
                        if (context.ActionArguments["membershipRequest"] is RequestUploadViewModel membershipRequestToConsider)
                        {
                            // A membership request belonging to the currently logged in student
                            var is_Owner = membershipRequestToConsider.Username.EqualsIgnoreCase(user_name);
                            if (is_Owner)
                                return true;
                        }
                        // No one should be able to add requests on behalf of another person.
                        return false;
                    }
                case Resource.STUDENT:
                    return false; // No one should be able to add students through this API
                case Resource.ADVISOR:
                    // User is admin
                    if (user_groups.Contains(AuthGroup.SiteAdmin))
                        return true;
                    else
                        return false; // Only super admin can add Advisors through this API
                case Resource.HOUSING_ADMIN:
                    //only superadmins can add a HOUSING_ADMIN
                    return false;
                case Resource.HOUSING:
                    {
                        // The user must be a student and not a member of an existing application
                        if (user_groups.Contains(AuthGroup.Student))
                        {
                            var sess_cde = Helpers.GetCurrentSession(_CCTContext);
                            var housingService = new HousingService(_CCTContext);
                            int? applicationID = housingService.GetApplicationID(user_name, sess_cde);
                            if (!applicationID.HasValue || applicationID == 0)
                            {
                                return true;
                            }
                            return false;
                        }
                        return false;
                    }
                case Resource.ADMIN:
                    return false;
                case Resource.ERROR_LOG:
                    return true;
                case Resource.NEWS:
                    return true;
                case Resource.RECIM_PARTICIPANT_ADMIN:
                     //fallthrough
                case Resource.RECIM_AFFILIATION:
                    //fallthrough
                case Resource.RECIM_ACTIVITY:
                    //fallthrough
                case Resource.RECIM_SERIES:
                    //fallthrough
                case Resource.RECIM_MATCH:
                    //fallthrough
                case Resource.RECIM_SURFACE:
                    //fallthrough
                case Resource.RECIM_SPORT:
                    {
                        return _recimParticipantService.IsAdmin(user_name);
                    }
                default: return false;
            }
        }
        private async Task<bool> CanUpdateAsync(string resource)
        {
            switch (resource)
            {
                case Resource.SHIFT:
                    {
                        if (user_groups.Contains(AuthGroup.Student))
                            return true;
                        return false;
                    }
                case Resource.MEMBERSHIP:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;
                        if (context.ActionArguments["membershipID"] is int membershipID)
                        {
                            var membershipToConsider = _membershipService.GetMembershipViewById(membershipID);
                            var activityCode = membershipToConsider.ActivityCode;
                            var sessionCode = membershipToConsider.SessionCode;


                            var userMembership = _membershipService
                                .GetMemberships(activityCode: activityCode, username: user_name, sessionCode: sessionCode)
                                .FirstOrDefault();
                            if (membershipToConsider.Participation == Participation.Advisor.GetCode())
                            {
                                return userMembership?.Participation == Participation.Advisor.GetCode();
                            }
                            else if (userMembership?.GroupAdmin == true && membershipToConsider.Participation != Participation.Advisor.GetCode())
                            {
                                // Activity Advisors can update memberships of people in their activity.
                                return true;
                            }


                            var is_membershipOwner = membershipToConsider.Username.EqualsIgnoreCase(user_name);
                            if (is_membershipOwner)
                            {
                                // Restrict what a regular owner can edit.
                                var originalMembership = _membershipService.GetSpecificMembership(membershipToConsider.MembershipID);
                                // If they are not trying to change their participation level, then it is ok
                                if (originalMembership.Participation == membershipToConsider.Participation)
                                    return true;
                            }
                        }


                        return false;
                    }

                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // Once a request is sent, no one is able to edit its contents.
                        // If a mistake is made in creating the original request, the user can delete it and make a new one.
                        if (context.ActionArguments["membershipRequestID"] is int mrID)
                        {
                            // Get the view model from the repository
                            var activityCode = _membershipRequestService.Get(mrID).ActivityCode;

                            // If user is a leader or advisor of the activity the request is for
                            return _membershipService
                                    .GetMemberships(
                                        activityCode: activityCode,
                                        username: user_name,
                                        participationTypes: new List<string>
                                           {
                                           Participation.Leader.GetCode(),
                                           Participation.Advisor.GetCode()
                                           })
                                    .Any();
                        }
                        return false;
                    }
                case Resource.MEMBERSHIP_PRIVACY:
                    {
                        if (context.ActionArguments["membershipID"] is int membershipID)
                        {
                            var membershipToConsider = _membershipService.GetSpecificMembership(membershipID);
                            var is_membershipOwner = membershipToConsider.Username.EqualsIgnoreCase(user_name);
                            if (is_membershipOwner)
                                return true;
                        }

                        return false;
                    }
                case Resource.STUDENT:
                    return false; // No one should be able to update a student through this API
                case Resource.HOUSING:
                    {
                        // The housing admins can update the application information (i.e. probation, offcampus program, etc.)
                        // If the user is a student, then the user must be on an application and be an editor to update the application
                        HousingService housingService = new HousingService(_CCTContext);
                        if (user_groups.Contains(AuthGroup.HousingAdmin))
                        {
                            return true;
                        }
                        else if (user_groups.Contains(AuthGroup.Student))
                        {
                            var sess_cde = Helpers.GetCurrentSession(_CCTContext);
                            int? applicationID = housingService.GetApplicationID(user_name, sess_cde);
                            if (context.ActionArguments["applicationID"] is int requestedApplicationID
                                && applicationID is not null
                                && applicationID == requestedApplicationID)
                            {
                                string editorUsername = housingService.GetEditorUsername(applicationID.Value);
                                return editorUsername.EqualsIgnoreCase(user_name);
                            }
                        }
                        return false;
                    }
                case Resource.PROFILE:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;

                        if (context.ActionArguments["username"] is string profile_username)
                            return profile_username.EqualsIgnoreCase(user_name);

                        return false;
                    }
                case Resource.PROFILE_PRIVACY:
                    {
                        // current implementation only allows for facstaff implementation. 
                        return user_groups.Contains(AuthGroup.FacStaff);
                    }
                case Resource.ACTIVITY_INFO:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;

                        if (context.ActionArguments["id"] is string activityCode)
                        {
                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            return isGroupAdmin;
                        }
                        return false;

                    }

                case Resource.ACTIVITY_STATUS:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;

                        if (context.ActionArguments["id"] is string activityCode)
                        {
                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            if (isGroupAdmin && context.ActionArguments["sess_cde"] is string sessionCode)
                            {
                                var activityService = context.HttpContext.RequestServices.GetRequiredService<Services.IActivityService>();
                                // If an activity is currently open, then a group admin has the ability to close it
                                return activityService.IsOpen(activityCode, sessionCode);
                            }
                        }

                        // If an activity is currently closed, only super admin has permission to edit its closed/open status   
                        return false;
                    }
                case Resource.EMERGENCY_CONTACT:
                    {
                        return context.ActionArguments["username"] is string emergency_contact_username 
                            && emergency_contact_username.EqualsIgnoreCase(user_name);
                    }

                case Resource.NEWS:
                    {
                        if (context.ActionArguments["newsID"] is int newsID)
                        {
                            var newsItem = _newsService.Get(newsID);
                            // only unapproved posts may be updated
                            if (newsItem.Accepted != false)
                                return false;

                            // can update if user is a Student News Admin
                            if (user_groups.Contains(AuthGroup.NewsAdmin))
                                return true;

                            // can update if user is news item author
                            return newsItem.ADUN.EqualsIgnoreCase(user_name);
                        }

                        return false;
                    }

                case Resource.NEWS_APPROVAL:
                    {
                        // can approve or deny if user is a Student News Admin
                        if (user_groups.Contains(AuthGroup.NewsAdmin))
                            return true;

                        return false;
                    }

                case Resource.RECIM_PARTICIPANT:
                    if (context.ActionArguments["username"] is string participant_username)
                        return participant_username.EqualsIgnoreCase(user_name);
                    return false;
                case Resource.RECIM_PARTICIPANT_ADMIN:
                    //fallthrough
                case Resource.RECIM_ACTIVITY:
                    //fallthrough
                case Resource.RECIM_AFFILIATION:
                    //fallthrough
                case Resource.RECIM_SERIES:
                    //fallthrough
                case Resource.RECIM_SURFACE:
                    //fallthrough
                case Resource.RECIM_SPORT:
                    {
                        return _recimParticipantService.IsAdmin(user_name);
                    }

                case Resource.RECIM_TEAM:
                    {
                        if(context.ActionArguments.TryGetValue("teamID",out object? teamID_object) && teamID_object is int teamID)
                        {
                            return _recimTeamService.IsTeamCaptain(user_name, teamID) || _recimParticipantService.IsAdmin(user_name);
                        }
                        return false;
                    }

                case Resource.RECIM_MATCH:
                    {
                        if (context.ActionArguments.TryGetValue("matchID", out object? matchID_Object) && matchID_Object is int matchID)
                        {
                            // if admin
                            if (_recimParticipantService.IsAdmin(user_name)) return true;

                            //if ref
                            var activityID = _CCTContext.Match.Find(matchID).Series.ActivityID;
                            return _recimActivityService.IsReferee(user_name, activityID);
                        }
                        return false;
                    }
                case Resource.RECIM_SUPER_ADMIN:
                    return user_groups.Contains(AuthGroup.RecIMSuperAdmin);
                default: return false;
            }
        }
        private async Task<bool> CanDeleteAsync(string resource)
        {
            switch (resource)
            {
                case Resource.SHIFT:
                    if (user_groups.Contains(AuthGroup.Student))
                        return true;
                    return false;
                case Resource.MEMBERSHIP:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;
                        if (context.ActionArguments["membershipID"] is int membershipID)
                        {
                            var membershipToConsider = _membershipService.GetSpecificMembership(membershipID);
                            var is_membershipOwner = membershipToConsider.Username.EqualsIgnoreCase(user_name);
                            if (is_membershipOwner)
                                return true;

                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: membershipToConsider.ActivityCode,
                                    username: user_name,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            return isGroupAdmin;
                        }

                        return false;
                    }
                case Resource.MEMBERSHIP_REQUEST:
                    {
                        // User is admin
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;
                        // membershipRequest = mr
                        if (context.ActionArguments["membershipRequestID"] is int mrID)
                        {
                            var mrToConsider = _membershipRequestService.Get(mrID);
                            var is_mrOwner = mrToConsider.Username.EqualsIgnoreCase(user_name);
                            if (is_mrOwner)
                                return true;

                            var activityCode = mrToConsider.ActivityCode;

                            var isGroupAdmin = _membershipService
                                .GetMemberships(
                                    activityCode: activityCode,
                                    username: user_name,
                                    participationTypes: new List<string> { Participation.GroupAdmin.GetCode() })
                                .Any();
                            if (isGroupAdmin)
                                return true;
                        }

                        return false;
                    }
                case Resource.STUDENT:
                    return false; // No one should be able to delete a student through our API
                case Resource.HOUSING:
                    {
                        // The housing admins can update the application information (i.e. probation, offcampus program, etc.)
                        // If the user is a student, then the user must be on an application and be an editor to update the application
                        HousingService housingService = new HousingService(_CCTContext);
                        if (user_groups.Contains(AuthGroup.HousingAdmin))
                        {
                            return true;
                        }
                        else if (user_groups.Contains(AuthGroup.Student))
                        {
                            var sess_cde = Helpers.GetCurrentSession(_CCTContext);
                            int? applicationID = housingService.GetApplicationID(user_name, sess_cde);
                            if (context.ActionArguments["applicationID"] is int requestedApplicationID && applicationID is not null && applicationID.Value == requestedApplicationID)
                            {
                                var editorUsername = housingService.GetEditorUsername(applicationID.Value);
                                return editorUsername.EqualsIgnoreCase(user_name);
                            }
                        }
                        return false;
                    }
                case Resource.ADMIN:
                    return false;
                case Resource.HOUSING_ADMIN:
                    {
                        // Only the superadmins can remove a housing admin from the whitelist
                        // Super admins have unrestricted access by default: no need to check
                        return false;
                    }
                case Resource.NEWS:
                    {
                        if (context.ActionArguments["newsID"] is int newsID)
                        {
                            var newsItem = _newsService.Get(newsID);

                            // only expired news items may be deleted
                            var newsDate = newsItem.Entered;
                            if (!newsDate.HasValue || DateTime.Now.Subtract(newsDate.Value).Days >= 14)
                            {
                                return false;
                            }

                            // can update if user is a Student News Admin
                            if (user_groups.Contains(AuthGroup.NewsAdmin))
                                return true;

                            // can update if user is news item author
                            return newsItem.ADUN.EqualsIgnoreCase(user_name);
                        }

                        return false;
                    }
                case Resource.SLIDER:
                    {
                        if (user_groups.Contains(AuthGroup.SiteAdmin))
                            return true;
                        return false;
                    }
                case Resource.RECIM_ACTIVITY:
                    //fallthrough
                case Resource.RECIM_AFFILIATION:
                    //fallthrough
                case Resource.RECIM_SERIES:
                    //fallthrough
                case Resource.RECIM_SPORT:
                    //fallthrough
                case Resource.RECIM_TEAM:
                    //fallthrough
                case Resource.RECIM_SURFACE:
                    //fallthrough
                case Resource.RECIM_MATCH:
                    return _recimParticipantService.IsAdmin(user_name);
                default: return false;
            }
        }


    }
}