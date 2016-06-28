using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using CCT_App.Services;
using CCT_App.Repositories;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/memberships")]
    public class MembershipsController : ApiController
    {

        private IMembershipService _membershipService;

        public MembershipsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _membershipService = new MembershipService(_unitOfWork);
        }
        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _membershipService.GetAll();
            return Ok(all);
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result =  _membershipService.Get(id);

            if( result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<controller>
        [HttpPost]
        [Route("", Name="Memberships")]
        public IHttpActionResult Post([FromBody] Membership membership)
        {
            if(!ModelState.IsValid || membership == null)
            {
                return BadRequest();
            }

            var result = _membershipService.Add(membership);

            if ( result == null)
            {
                return NotFound();
            }

            return Created("memberships", membership);

        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("")]
        public IHttpActionResult Put(int id, [FromBody]Membership membership)
        {
            if (!ModelState.IsValid || membership == null || id != membership.MEMBERSHIP_ID)
            {
                return BadRequest();
            }

            var result = _membershipService.Update(id, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membership);
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var result = _membershipService.Delete(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}