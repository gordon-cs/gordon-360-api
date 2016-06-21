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
        private string invalidModelStateMsg = "Model State is invalid!";

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<Membership> Get()
        {
            return cct_db_context.Memberships;
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(int id)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, invalidModelStateMsg);
            }
            var result =  cct_db_context.Memberships.Find(id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage Post([FromBody]Membership membership)
        {
            membership.MEMBERSHIP_ID = cct_db_context.Memberships.Count();
            cct_db_context.Memberships.Add(membership);

            var msg = Request.CreateResponse(HttpStatusCode.Created);
            msg.Headers.Location = new Uri(Request.RequestUri + membership.MEMBERSHIP_ID.ToString());
            return msg;

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