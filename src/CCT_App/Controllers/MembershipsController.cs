using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using cct_api.models;

namespace cct_api.controllers
{
    [Route("api/[controller]")]
    public class MembershipsController : Controller
    {
        public IMembershipRepository Memberships;
        public MembershipsController(IMembershipRepository memberships)
        {
            Memberships = memberships;
        }
        [RouteAttribute("")]
        public IEnumerable<Membership> GetAll()
        {
            return Memberships.GetAll();
        }

        [HttpGetAttribute("{id}")]
        public IActionResult GetById(string id)
        {
            var membership = Memberships.Find(id);
            if (membership == null)
            {
                return NotFound();
            }
            return new ObjectResult(membership);
        }

        [HttpPostAttribute]
        public IActionResult Create([FromBodyAttribute] Membership mbrshp)
        {
            if (mbrshp == null)
            {
                return BadRequest();
            }
            Memberships.Add(mbrshp);
            return CreatedAtRoute("GetMembership", new { controller = "Memberships", id = mbrshp.membership_id}, mbrshp);
        }

        [HttpPutAttribute("{id}")]
        public IActionResult Update(string id, [FromBodyAttribute] Membership mbrshp)
        {
            if (mbrshp == null || mbrshp.membership_id != id)
            {
                return BadRequest();
            }

            var membership = Memberships.Find(id);
            if ( membership == null)
            {
                return NotFound();
            }

            Memberships.Update(mbrshp);
            return new NoContentResult();
        }

        [HttpDeleteAttribute("{id}")]
        public void Delete(string id)
        {
            Memberships.Remove(id);
        }
    }
}