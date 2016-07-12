using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gordon360.Models;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/requests")]
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
        /// Gets the memberships requests for the specified student
        /// </summary>
        /// <param name="id">The student ID</param>
        /// <returns>All membership requests associated with the student</returns>
        [HttpGet]
        [Route("student/{id}")]
        public IHttpActionResult GetMembershipsRequestsForStudent(string id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _membershipRequestService.GetMembershipRequestsForStudent(id);

            if(result == null)
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
        public IHttpActionResult Post(Request membershipRequest)
        {
            if( !ModelState.IsValid || membershipRequest == null)
            {
                return BadRequest();
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
        public IHttpActionResult Put(int id, Request membershipRequest)
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
        /// Delets a membership request
        /// </summary>
        /// <param name="id">The id of the membership request to delete</param>
        /// <returns>The deleted object</returns>
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
