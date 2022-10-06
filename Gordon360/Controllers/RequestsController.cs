using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class RequestsController : GordonControllerBase
    {
        public IMembershipRequestService _membershipRequestService;

        public RequestsController(CCTContext context, IMembershipRequestService membershipRequestService)
        {
            _membershipRequestService = membershipRequestService;
        }

        /// <summary>
        /// Gets all Membership Request Objects
        /// </summary>
        /// <returns>List of all requests for membership</returns>
        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<IEnumerable<MembershipRequestViewModel>> Get()
        {
            return Ok(_membershipRequestService.GetAll());
        }

        /// <summary>
        ///  Gets a specific Membership Request Object
        /// </summary>
        /// <param name="id">The ID of the membership request</param>
        /// <returns>A memberships request with the specified id</returns>
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<MembershipRequestViewModel> Get(int id)
        {
            var result = _membershipRequestService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// Gets the memberships requests for the specified activity
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns>All membership requests associated with the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipView>> GetMembershipRequestsByActivity(string activityCode)
        {
            var result = _membershipRequestService.GetMembershipRequestsByActivity(activityCode);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the memberships requests for the person making the request
        /// </summary>
        /// <returns>All membership requests associated with the student</returns>
        [HttpGet]
        [Route("current-user")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_STUDENT)]
        public ActionResult<IEnumerable<MembershipView>> GetMembershipsRequestsForCurrentUser()
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var result = _membershipRequestService.GetMembershipRequestsByUsername(authenticatedUserUsername);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new membership request
        /// </summary>
        /// <param name="membershipRequest">The request to be added</param>
        /// <returns>The added request if successful. HTTP error message if not.</returns>
        [HttpPost]
        [Route("", Name = "membershipRequest")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> PostAsync([FromBody] RequestUploadViewModel membershipRequest)
        {
            var result = await _membershipRequestService.AddAsync(membershipRequest);

            if (result == null)
            {
                return NotFound();
            }

            return Created("membershipRequest", result);
        }

        /// <summary>
        /// Updates a membership request
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <param name="membershipRequest">The updated membership request object</param>
        /// <returns>The updated request if successful. HTTP error message if not.</returns>
        [HttpPut]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> PutAsync(int id, RequestUploadViewModel membershipRequest)
        {
            var result = await _membershipRequestService.UpdateAsync(id, membershipRequest);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membershipRequest);
        }


        /// <summary>
        /// Sets a membership request to Approved
        /// </summary>
        /// <param name="id">The id of the membership request in question.</param>
        /// <returns>If successful: THe updated membership request wrapped in an OK Http status code.</returns>
        [HttpPost]
        [Route("{id}/approve")]
        [StateYourBusiness(operation = Operation.DENY_ALLOW, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<MembershipView>> ApproveAsync(int id)
        {
            var result = await _membershipRequestService.ApproveAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Sets the membership request to Denied
        /// </summary>
        /// <param name="id">The id of the membership request in question.</param>
        /// <returns>If successful: The updated membership request wrapped in an OK Http status code.</returns>
        [HttpPost]
        [Route("{id}/deny")]
        [StateYourBusiness(operation = Operation.DENY_ALLOW, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> DenyAsync(int id)
        {
            var result = await _membershipRequestService.DenyAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes a membership request
        /// </summary>
        /// <param name="id">The id of the membership request to delete</param>
        /// <returns>The deleted object</returns>
        [HttpDelete]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> Delete(int id)
        {
            var result = await _membershipRequestService.DeleteAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
