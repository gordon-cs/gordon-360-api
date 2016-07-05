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
using CCT_App.Models.ViewModels;

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
        public IHttpActionResult Get()
        {
            
            var result = _membershipService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Get a single membership based on the id given
        /// </summary>
        /// <param name="id">The id of a membership within the database</param>
        /// <remarks>Queries the database about the specified membership</remarks>
        /// <returns>The information about one specific membership</returns>
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

            MembershipViewModel membership = result;
            return Ok(membership);
        }

        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="membership">The membership item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new membership to the server to be added into the database</remarks>
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

        /// <summary>Update an existing membership item</summary>
        /// <param name="id">The membership id of whichever one is to be changed</param>
        /// <param name="membership">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
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

        /// <summary>Delete an existing membership</summary>
        /// <param name="id">The identifier for the membership to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given membership from the database</remarks>
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
