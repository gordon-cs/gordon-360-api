using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class MembershipsController : GordonControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IAccountService _accountService;
        private readonly IActivityService _activityService;
        private readonly IRoleCheckingService _roleCheckingService;

        public MembershipsController(CCTContext context)
        {
            _membershipService = new MembershipService(context);
            _accountService = new AccountService(context);
            _activityService = new ActivityService(context);
            _roleCheckingService = new RoleCheckingService(context);
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
        public ActionResult<IEnumerable<MembershipViewModel>> Get()
        {
            var result = _membershipService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Get all the memberships associated with a given activity
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetMembershipsForActivity(string id)
        {
            var result = _membershipService.GetMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the group admin memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/group-admin")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.GROUP_ADMIN_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetGroupAdminForActivity(string id)
        {
            var result = _membershipService.GetGroupAdminMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the leader-type memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/leaders")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.LEADER_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetLeadersForActivity(string id)
        {
            var result = _membershipService.GetLeaderMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the advisor-type memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all advisor-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/advisors")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ADVISOR_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetAdvisorsForActivity(string id)
        {
            var result = _membershipService.GetAdvisorMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/followers")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityFollowersCount(string id)
        {
            var result = _membershipService.GetActivityFollowersCount(id);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (besides followers) of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/members")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityMembersCount(string id)
        {
            var result = _membershipService.GetActivityMembersCount(id);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/followers/{sess_cde}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityFollowersCountForSession(string id, string sess_cde)
        {
            var result = _membershipService.GetActivityFollowersCountForSession(id, sess_cde);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (excluding followers) of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/members/{sess_cde}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public ActionResult<int> GetActivityMembersCountForSession(string id, string sess_cde)
        {
            var result = _membershipService.GetActivityMembersCountForSession(id, sess_cde);

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
        public ActionResult<MEMBERSHIP> Post([FromBody] MEMBERSHIP membership)
        {
            var result = _membershipService.Add(membership);

            if (result == null)
            {
                return NotFound();
            }

            return Created("memberships", membership);

        }

        /// <summary>Fetch memberships that a specific student has been a part of</summary>
        /// <param name="id">The Student id</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/{id}")]
        [HttpGet]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_STUDENT)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetMembershipsForStudent(string id)
        {
            var result = _membershipService.GetMembershipsForStudent(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        /// <summary>
        /// Fetch memberships that a specific student has been a part of
        /// @TODO: Move security checks to state your business? Or consider changing implementation here
        /// </summary>
        /// <param name="username">The Student Username</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/username/{username}")]
        [HttpGet]
        public async Task<ActionResult<List<MembershipViewModel>>> GetMembershipsForStudentByUsename(string username)
        {
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var result = await _membershipService.GetMembershipsForStudent(id);

            if (result == null)
            {
                return NotFound();
            }
            // privacy control of membership view model
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            if (viewerType == Position.SUPERADMIN || viewerType == Position.POLICE)              //super admin and gordon police reads all
                return Ok(result);
            else
            {
                List<MembershipViewModel> list = new List<MembershipViewModel>();
                foreach (var item in result)
                {
                    var act = _activityService.Get(item.ActivityCode);
                    var admins = await _membershipService.GetGroupAdminMembershipsForActivity(item.ActivityCode);
                    bool groupAdmin = false;
                    foreach (var admin in admins)               // group admin of a group can read membership of this group
                    {
                        if (admin.AD_Username.Equals(authenticatedUserUsername))
                            groupAdmin = true;
                    }
                    if (groupAdmin)
                    {
                        list.Add(item);
                    }
                    else if (act.Privacy != true)               // check group privacy
                    {
                        if (item.Privacy != true)               // check personal membership privacy
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
        public ActionResult<MEMBERSHIP> Put(int id, [FromBody] MEMBERSHIP membership)
        {
            var result = _membershipService.Update(id, membership);

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
        public ActionResult<MEMBERSHIP> ToggleGroupAdmin([FromBody] MEMBERSHIP membership)
        {
            var id = membership.MEMBERSHIP_ID;

            var result = _membershipService.ToggleGroupAdmin(id, membership);

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
