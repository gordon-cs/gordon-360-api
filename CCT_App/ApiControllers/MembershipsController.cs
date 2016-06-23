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

        public MembershipsController(CCTEntities dbContext)
        {
            database = dbContext;
        }

        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IEnumerable<Membership> Get()
        {
            return database.Memberships;
        }

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

        // PUT api/<controller>/5
        [HttpPut]
        [Route("")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
        }
    }
}