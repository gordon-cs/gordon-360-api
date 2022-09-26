using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class MembershipsController : GordonControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IAccountService _accountService;
        private readonly IActivityService _activityService;

        public MembershipsController(CCTContext context, IActivityService activityService, IAccountService accountService, IMembershipService membershipService)
        {
            _activityService = activityService;
            _accountService = accountService;
            _membershipService = membershipService;
        }

        /// <summary>
        /// Get all the memberships associated with a given activity
        /// </summary>
        /// <param name="activityCode">The activity ID</param>
        /// <param name="sessionCode">Optional code of session to get for</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("activity/{activityCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetMembershipsForActivity(string activityCode, string? sessionCode)
        {
            var result = _membershipService.GetMembershipsForActivity(activityCode, sessionCode);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the group admin memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{activityCode}/group-admin")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.GROUP_ADMIN_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetGroupAdminForActivity(string activityCode)
        {
            var result = _membershipService.GetGroupAdminMembershipsForActivity(activityCode);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the leader-type memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{activityCode}/leaders")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.LEADER_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetLeadersForActivity(string activityCode)
        {
            var result = _membershipService.GetLeaderMembershipsForActivity(activityCode);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the advisor-type memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>A list of all advisor-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{activityCode}/advisors")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ADVISOR_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetAdvisorsForActivityAsync(string activityCode)
        {
            var result = _membershipService.GetAdvisorMembershipsForActivity(activityCode);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}/followers")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityFollowersCount(string activityCode)
        {
            var result = _membershipService.GetActivityFollowersCount(activityCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (besides followers) of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}/members")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityMembersCount(string activityCode)
        {
            var result = _membershipService.GetActivityMembersCount(activityCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}/followers/{sessionCode}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityFollowersCountForSession(string activityCode, string sessionCode)
        {
            var result = _membershipService.GetActivityFollowersCountForSession(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (excluding followers) of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}/members/{sessionCode}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityMembersCountForSession(string activityCode, string sessionCode)
        {
            var result = _membershipService.GetActivityMembersCountForSession(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="membership">The membership item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new membership to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name = "Memberships")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> PostAsync([FromBody] MembershipUploadViewModel membership)
        {
            var idNum = _accountService.GetAccountByUsername(membership.Username).GordonID;

            if (idNum == null)
            {
                return NotFound();
            }

            var result = await _membershipService.AddAsync(membership);

            if (result == null)
            {
                return NotFound();
            }

            return Created("memberships", result);

        }

        /// <summary>
        /// Fetch memberships that a specific student has been a part of
        /// @TODO: Move security checks to state your business? Or consider changing implementation here
        /// </summary>
        /// <param name="username">The Student Username</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/{username}")]
        [HttpGet]
        public ActionResult<List<MembershipView>> GetMembershipsForStudentByUsername(string username)
        {
            var result = _membershipService.GetMembershipsForStudent(username);

            if (result == null)
            {
                return NotFound();
            }
            // privacy control of membership view model
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var viewerGroups = AuthUtils.GetGroups(User);

            if (username == authenticatedUserUsername || viewerGroups.Contains(AuthGroup.SiteAdmin) || viewerGroups.Contains(AuthGroup.Police))              //super admin and gordon police reads all
                return Ok(result);
            else
            {
                List<MembershipView> list = new List<MembershipView>();
                foreach (var item in result)
                {
                    var act = _activityService.Get(item.ActivityCode);
                    if (!(act.Privacy == true || item.Privacy == true))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        // If the current authenticated user is an admin of this group, then include the membership
                        var admins = _membershipService.GetGroupAdminMembershipsForActivity(item.ActivityCode);
                        if (admins.Any(a => a.Username == authenticatedUserUsername))
                        {
                            list.Add(item);
                        }
                    }
                }
                return Ok(list);
            }
        }

        /// <summary>Update an existing membership item</summary>
        /// <param name="membershipID">The membership id of whichever one is to be changed</param>
        /// <param name="membership">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        /// <returns>The membership information that the student is a part of</returns>
        // PUT api/<controller>/5
        [HttpPut]
        [Route("{membershipID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> PutAsync(int membershipID, [FromBody] MembershipUploadViewModel membership)
        {
            var result = await _membershipService.UpdateAsync(membershipID, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>Update an existing membership item to be a group admin or not</summary>
        ///  /// <param name="membershipID">The content within the membership that is to be changed</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{membershipID}/group-admin/{isGroupAdmin}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> SetGroupAdminAsync(int membershipID, bool isGroupAdmin)
        {
            var result = await _membershipService.SetGroupAdminAsync(membershipID, isGroupAdmin);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>Update an existing membership item to be private or not</summary>
        /// <param name="membership">The membership to toggle privacy on</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{membershipID}/privacy/{isPrivate}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_PRIVACY)]
        public async Task<ActionResult<MembershipView>> SetPrivacyAsync(int membershipID, bool isPrivate)
        {

            var updatedMembership = await _membershipService.SetPrivacyAsync(membershipID, isPrivate);
            return Ok(updatedMembership);
        }

        /// <summary>Delete an existing membership</summary>
        /// <param name="id">The identifier for the membership to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given membership from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP)]
        public ActionResult<MEMBERSHIP> Delete(int id)
        {
            var result = _membershipService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>	
        /// Determines whether or not the given student is a Group Admin of some activity	
        /// </summary>
        /// <param name="username">The account username to check</param>
        [HttpGet]
        [Route("isGroupAdmin/{id}")]
        public ActionResult<bool> IsGroupAdmin(string username)
        {
            var result = _membershipService.IsGroupAdmin(username);

            return Ok(result);
        }
    }
}
