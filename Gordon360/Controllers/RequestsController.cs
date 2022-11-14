using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gordon360.Exceptions;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class RequestsController : GordonControllerBase
    {
        public IMembershipRequestService _membershipRequestService;

        public RequestsController(IMembershipRequestService membershipRequestService)
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
        public ActionResult<IEnumerable<RequestView>> Get()
        {
            return Ok(_membershipRequestService.GetAll());
        }

        /// <summary>
        ///  Gets a specific Membership Request Object
        /// </summary>
        /// <param name="id">The ID of the membership request</param>
        /// <returns>A RequestView that matches the specified id</returns>
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<RequestView> Get(int id)
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
        /// <param name="activityCode">The activity code</param>
        /// <param name="sessionCode">The session code</param>
        /// <param name="requestStatus">The optional status of the requests to search</param>
        /// <returns>All membership requests associated with the activity</returns>
        [HttpGet]
        [Route("activity/{activityCode}/session/{sessionCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY)]
        public ActionResult<IEnumerable<RequestView>> GetMembershipRequestsByActivity(string activityCode, string sessionCode, string? requestStatus = null)
        {
            IEnumerable<RequestView> result = _membershipRequestService.GetMembershipRequests(activityCode, sessionCode, requestStatus);

            return Ok(result);
        }

        /// <summary>
        /// Gets the memberships requests for the person making the request
        /// </summary>
        /// <returns>All membership requests associated with the current user</returns>
        [HttpGet]
        [Route("users/current")]
        public ActionResult<IEnumerable<RequestView>> GetMembershipsRequestsForCurrentUser()
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
        /// <param name="membershipRequestID">The membership request id</param>
        /// <param name="membershipRequest">The updated membership request object</param>
        /// <returns>The updated request if successful. HTTP error message if not.</returns>
        [HttpPut]
        [Route("{membershipRequestID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView?>> PutAsync(int membershipRequestID, RequestUploadViewModel membershipRequest)
        {
            var result = await _membershipRequestService.UpdateAsync(membershipRequestID, membershipRequest);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membershipRequest);
        }


        /// <summary>
        /// Sets a membership request to Approved
        /// </summary>
        /// <param name="membershipRequestID">The id of the membership request in question.</param>
        /// <param name="status">The status that the membership requst will be changed to.</param>
        /// <returns>The updated request</returns>
        [HttpPost]
        [Route("{membershipRequestID}/status")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> UpdateStatusAsync(int membershipRequestID, [FromBody] string status)
        {
            RequestView? updated = status switch
            {
                Request_Status.APPROVED => await _membershipRequestService.ApproveAsync(membershipRequestID),
                Request_Status.DENIED => await _membershipRequestService.DenyAsync(membershipRequestID),
                Request_Status.PENDING => await _membershipRequestService.SetPendingAsync(membershipRequestID),
                _ => throw new BadInputException() { ExceptionMessage = $"Status must be one of '{Request_Status.APPROVED}', '{Request_Status.DENIED}', or '{Request_Status.PENDING}'" }
            };

            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Deletes a membership request
        /// </summary>
        /// <param name="membershipRequestID">The id of the membership request to delete</param>
        /// <returns>The deleted request as a RequestView</returns>
        [HttpDelete]
        [Route("{membershipRequestID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP_REQUEST)]
        public async Task<ActionResult<RequestView>> Delete(int membershipRequestID)
        {
            var result = await _membershipRequestService.DeleteAsync(membershipRequestID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
