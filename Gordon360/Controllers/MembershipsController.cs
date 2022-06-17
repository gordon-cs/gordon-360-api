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
        private readonly IActivityService _activityService;

        public MembershipsController(CCTContext context)
        {
            _membershipService = new MembershipService(context);
            _activityService = new ActivityService(context);
        }

        /// <summary>
        /// Get all memberships
        /// </summary>
        /// <returns>
        /// A list of all memberships
        /// </returns>
        /// <remarks>
        /// Server makes call to the database and returns all current memberships
        /// </remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<IEnumerable<MembershipViewModel>>> GetAsync()
        {
            var result = await _membershipService.GetAllAsync();
            return Ok(result);
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
        public async Task<ActionResult<IEnumerable<MembershipViewModel>>> GetMembershipsForActivityAsync(string activityCode, string? sessionCode)
        {
            var result = await _membershipService.GetMembershipsForActivityAsync(activityCode, sessionCode);
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
        public async Task<ActionResult<IEnumerable<MembershipViewModel>>> GetGroupAdminForActivityAsync(string activityCode)
        {
            var result = await _membershipService.GetGroupAdminMembershipsForActivityAsync(activityCode);

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
        public async Task<ActionResult<IEnumerable<MembershipViewModel>>> GetLeadersForActivityAsync(string activityCode)
        {
            var result = await _membershipService.GetLeaderMembershipsForActivityAsync(activityCode);

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
        public async Task<ActionResult<IEnumerable<MembershipViewModel>>> GetAdvisorsForActivityAsync(string activityCode)
        {
            var result = await _membershipService.GetAdvisorMembershipsForActivityAsync(activityCode);

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
        public async Task<ActionResult<int>> GetActivityFollowersCountAsync(string activityCode)
        {
            var result = await _membershipService.GetActivityFollowersCountAsync(activityCode);

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
        public async Task<ActionResult<int>> GetActivityMembersCountAsync(string activityCode)
        {
            var result = await _membershipService.GetActivityMembersCountAsync(activityCode);

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
        public async Task<ActionResult<int>> GetActivityFollowersCountForSessionAsync(string activityCode, string sessionCode)
        {
            var result = await _membershipService.GetActivityFollowersCountForSessionAsync(activityCode, sessionCode);

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
        public async Task<ActionResult<int>> GetActivityMembersCountForSessionAsync(string activityCode, string sessionCode)
        {
            var result = await _membershipService.GetActivityMembersCountForSessionAsync(activityCode, sessionCode);

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
        public async Task<ActionResult<MEMBERSHIP>> PostAsync([FromBody] MEMBERSHIP membership)
        {
            var result = await _membershipService.AddAsync(membership);

            if (result == null)
            {
                return NotFound();
            }

            return Created("memberships", membership);

        }

        /// <summary>
        /// Fetch memberships that a specific student has been a part of
        /// @TODO: Move security checks to state your business? Or consider changing implementation here
        /// </summary>
        /// <param name="username">The Student Username</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/{username}")]
        [HttpGet]
        public async Task<ActionResult<List<MembershipViewModel>>> GetMembershipsForStudentByUsenameAsync(string username)
        {
            var result = await _membershipService.GetMembershipsForStudentAsync(username);

            if (result == null)
            {
                return NotFound();
            }
            // privacy control of membership view model
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var viewerGroups = AuthUtils.GetGroups(User);

            if (viewerGroups.Contains(AuthGroup.SiteAdmin) || viewerGroups.Contains(AuthGroup.Police))              //super admin and gordon police reads all
                return Ok(result);
            else
            {
                List<MembershipViewModel> list = new List<MembershipViewModel>();
                foreach (var item in result)
                {
                    var act = _activityService.Get(item.ActivityCode);
                    if (!(act.Privacy == true || item.Privacy == true))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        var admins = await _membershipService.GetGroupAdminMembershipsForActivityAsync(item.ActivityCode);
                        if (admins.Any(a => a.AD_Username == authenticatedUserUsername))
                        {
                            list.Add(item);
                        }
                    }
                }
                return Ok(list);
            }
        }

        /// <summary>Update an existing membership item</summary>
        /// <param name="id">The membership id of whichever one is to be changed</param>
        /// <param name="membership">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        // PUT api/<controller>/5
        [HttpPut]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MEMBERSHIP>> PutAsync(int id, [FromBody] MEMBERSHIP membership)
        {
            var result = await _membershipService.UpdateAsync(id, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membership);
        }

        /// <summary>Update an existing membership item to be a group admin or not</summary>
        ///  /// <param name="membership">The content within the membership that is to be changed</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/group-admin")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public async Task<ActionResult<MEMBERSHIP>> ToggleGroupAdminAsync([FromBody] MEMBERSHIP membership)
        {
            var id = membership.MEMBERSHIP_ID;

            var result = await _membershipService.ToggleGroupAdminAsync(id, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>Update an existing membership item to be private or not</summary>
        /// <param name="id">The id of the membership</param>
        /// <param name = "p">the boolean value</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_PRIVACY)]
        public ActionResult TogglePrivacy(int id, bool p)
        {
            _membershipService.TogglePrivacy(id, p);
            return Ok();
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
        /// <param name="id">The student id</param>
        [HttpGet]
        [Route("isGroupAdmin/{id}")]
        public ActionResult<bool> IsGroupAdmin(int id)
        {
            var result = _membershipService.IsGroupAdmin(id);

            return Ok(result);
        }
    }
}
