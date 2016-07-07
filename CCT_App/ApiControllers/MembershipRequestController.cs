using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;
using CCT_App.Repositories;
using CCT_App.Services;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("KJzKJ6FOKx/api/requests")]
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
        /// <returns>IHttpActionResult with a list of requests</returns>
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
        /// <returns></returns>
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



    }
}
