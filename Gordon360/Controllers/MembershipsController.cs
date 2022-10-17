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

        public MembershipsController(IActivityService activityService, IAccountService accountService, IMembershipService membershipService)
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
        /// <returns>An IEnumerable of the matching MembershipViews</returns>
        [HttpGet]
        [Route("activities/{activityCode}/sessions/{sessionCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipView>> GetMembershipsForActivity(string activityCode, string? sessionCode)
        {
            var result = _membershipService.GetMembershipsForActivity(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the group admin memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>An IEnumerable of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activities/{activityCode}/sessions/{sessionCode}/admins")]
        public ActionResult<IEnumerable<MembershipView>> GetGroupAdminsForActivity(string activityCode, string sessionCode)
        {
            var result = _membershipService.GetGroupAdminMembershipsForActivity(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the leader-type memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>An IEnumerable of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{activityCode}/leaders")]
        public ActionResult<IEnumerable<MembershipView>> GetLeadersForActivity(string activityCode)
        {
            var result = _membershipService.GetLeaderMembershipsForActivity(activityCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the advisor-type memberships associated with a given activity.
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <returns>An IEnumerable of all advisor-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{activityCode}/advisors")]
        public ActionResult<IEnumerable<MembershipView>> GetAdvisorsForActivityAsync(string activityCode)
        {
            var result = _membershipService.GetAdvisorMembershipsForActivity(activityCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activities/{activityCode}/sessions/{sessionCode}/subscriber-count")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivitySubscribersCountForSession(string activityCode, string sessionCode)
        {
            var result = _membershipService.GetActivitySubscribersCountForSession(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (excluding followers) of an activity
        /// </summary>
        /// <param name="activityCode">The activity ID.</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activities/{activityCode}/sessions/{sessionCode}/member-count")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityMembersCountForSession(string activityCode, string sessionCode)
        {
            var result = _membershipService.GetActivityMembersCountForSession(activityCode, sessionCode);

            return Ok(result);
        }

        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="membershipUpload">The membership item containing all required and relevant information</param>
        /// <returns>The newly created membership as a MembershipView object</returns>
        /// <remarks>Posts a new membership to the server to be added into the database</remarks>
        [HttpPost]
        [Route("", Name = "Memberships")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> PostAsync([FromBody] MembershipUploadViewModel membershipUpload)
        {
            var idNum = _accountService.GetAccountByUsername(membershipUpload.Username).GordonID;

            var result = await _membershipService.AddAsync(membershipUpload);

            if (result == null)
            {
                return BadRequest();
            }

            return Created("memberships", result);

        }

        /// <summary>Update an existing membership item</summary>
        /// <param name="membershipID">The membership id of whichever one is to be changed</param>
        /// <param name="membership">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        /// <returns>The updated membership as a MembershipView object</returns>
        [HttpPut]
        [Route("{membershipID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> PutAsync(int membershipID, [FromBody] MembershipUploadViewModel membership)
        {
            var result = await _membershipService.UpdateAsync(membershipID, membership);

            return Ok(result);
        }

        /// <summary>Update an existing membership item to be a group admin or not</summary>
        /// <param name="membershipID">The content within the membership that is to be changed</param>
        /// <param name="isGroupAdmin">The new value of GroupAdmin</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        /// <returns>The updated membership as a MembershipView object</returns>
        [HttpPut]
        [Route("{membershipID}/group-admin")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MembershipView>> SetGroupAdminAsync(int membershipID, [FromBody] bool isGroupAdmin)
        {
            var result = await _membershipService.SetGroupAdminAsync(membershipID, isGroupAdmin);

            return Ok(result);
        }

        /// <summary>Update an existing membership item to be private or not</summary>
        /// <param name="membershipID">The membership to set the privacy of</param>
        /// <param name="isPrivate">The new value of Privacy for the membership</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        /// <returns>The updated membership as a MembershipView object</returns>
        [HttpPut]
        [Route("{membershipID}/privacy")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_PRIVACY)]
        public async Task<ActionResult<MembershipView>> SetPrivacyAsync(int membershipID, [FromBody] bool isPrivate)
        {

            var updatedMembership = await _membershipService.SetPrivacyAsync(membershipID, isPrivate);
            return Ok(updatedMembership);
        }

        /// <summary>Delete an existing membership</summary>
        /// <param name="membershipID">The identifier for the membership to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given membership from the database</remarks>
        /// <returns>The deleted membership as a MembershipView object</returns>
        [HttpDelete]
        [Route("{membershipID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP)]
        public ActionResult<MEMBERSHIP> Delete(int membershipID)
        {
            var result = _membershipService.Delete(membershipID);

            return Ok(result);
        }
    }
}
