using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gordon360.Models;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/requests")]
    [Authorize]
    [CustomExceptionFilter]
    public class MembershipRequestController : ApiController
    {
        public IMembershipRequestService _membershipRequestService;

        public MembershipRequestController()
        {
            var _unitOfWork = new UnitOfWork();
            _membershipRequestService = new MembershipRequestService(_unitOfWork);
        }

        public MembershipRequestController(IMembershipRequestService membershipRequestService)
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
        public IHttpActionResult Get()
        {
            var all = _membershipRequestService.GetAll();
            return Ok(all);
        }

        /// <summary>
        ///  Gets a specific Membership Request Object
        /// </summary>
        /// <param name="id">The ID of the membership request</param>
        /// <returns>A memberships request with the specified id</returns>
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP_REQUEST)]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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
        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY)]
        public IHttpActionResult GetMembershipsRequestsForActivity(string id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.GetMembershipRequestsForActivity(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the memberships requests for the specified student
        /// </summary>
        /// <param name="id">The student id</param>
        /// <returns>All membership requests associated with the student</returns>
        [HttpGet]
        [Route("student/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_STUDENT)]
        public IHttpActionResult GetMembershipsRequestsForStudent(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.GetMembershipRequestsForStudent(id);

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
        [Route("",Name = "membershipRequest")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP_REQUEST)]
        public IHttpActionResult Post(REQUEST membershipRequest)
        {
            if( !ModelState.IsValid || membershipRequest == null)
            {
                throw new BadInputException() { ExceptionMessage = ModelState.ToString() };
            }

            var result = _membershipRequestService.Add(membershipRequest);

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
        public IHttpActionResult Put(int id, REQUEST membershipRequest)
        {
            if (!ModelState.IsValid || membershipRequest == null || id != membershipRequest.REQUEST_ID)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.Update(id, membershipRequest);

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
        public IHttpActionResult ApproveRequest(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.ApproveRequest(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Sets the membership request to Denied
        /// </summary>
        /// <param name="id">The id of the membership reuqest in question.</param>
        /// <returns>If successful: THe updated membership request wrapped in an OK Http status code.</returns>
        [HttpPost]
        [Route("{id}/deny")]
        [StateYourBusiness(operation = Operation.DENY_ALLOW, resource = Resource.MEMBERSHIP_REQUEST)]
        public IHttpActionResult DenyRequest(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.DenyRequest(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        /// <summary>
        /// Delets a membership request
        /// </summary>
        /// <param name="id">The id of the membership request to delete</param>
        /// <returns>The deleted object</returns>
        [Route("{id}")]
        [HttpDelete]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP_REQUEST)]
        public IHttpActionResult Delete(int id)
        {
            var result = _membershipRequestService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
