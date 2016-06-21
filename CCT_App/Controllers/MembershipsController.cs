using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCT_App.Models;

namespace CCT_App.Controllers
{
    [RoutePrefix("api/memberships")]
    public class MembershipsController : ApiController
    {

        private CCTEntities cct_db_context = new CCTEntities();

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<Membership> Get()
        {
            return cct_db_context.Memberships;
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
            var result =  cct_db_context.Memberships.Find(id);
            return Ok(result);
        }

        // POST api/<controller>
        [HttpPost]
        [Route(Name="memberships")]
        public IHttpActionResult Post([FromBody] Membership membership)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            membership.MEMBERSHIP_ID = cct_db_context.Memberships.Count();
            cct_db_context.Memberships.Add(membership);
            cct_db_context.SaveChanges();

            var routeName = Request.RequestUri.ToString(); 
            var routeValue = membership.MEMBERSHIP_ID.ToString();
            return CreatedAtRoute<Membership>("memberships", membership.MEMBERSHIP_ID, membership);

        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}