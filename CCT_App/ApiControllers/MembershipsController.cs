using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;
using System.Data.Entity.Core.Objects;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/memberships")]
    public class MembershipsController : ApiController
    {

        private CCTEntities database = new CCTEntities();

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
        public IEnumerable<Membership> Get()
        {
            return database.Memberships;
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
        public IHttpActionResult GetMembership(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result =  database.Memberships.Find(id);

            if( result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="membership">The membership item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new membership to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name="memberships")]
        public IHttpActionResult Post([FromBody] Membership membership)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ObjectResult<ACTIVE_CLUBS_PER_SESS_ID_Result> valid_activity_codes = database.ACTIVE_CLUBS_PER_SESS_ID(membership.SESSION_CDE);
            bool offered = false;
            string potential_activity = membership.ACT_CDE.Trim();
            foreach (ACTIVE_CLUBS_PER_SESS_ID_Result activity in valid_activity_codes)
            {
                if(potential_activity.Equals(activity.ACT_CDE.Trim()))
                {
                    offered = true;
                }
            }

            if (!offered)
            {
                return NotFound();
            }

            database.Memberships.Add(membership);
            database.SaveChanges();

            var routeName = Request.RequestUri.ToString(); 
            var routeValue = membership.MEMBERSHIP_ID.ToString();
            return CreatedAtRoute<Membership>("memberships", membership.MEMBERSHIP_ID, membership);

        }

        /// <summary>Update an existing membership item</summary>
        /// <param name="id">The membership id of whichever one is to be changed</param>
        /// <param name="value">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        // PUT api/<controller>/5
        [HttpPut]
        [Route("")]
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>Delete an existing membership</summary>
        /// <param name="id">The identifier for the membership to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given membership from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
        }
    }
}