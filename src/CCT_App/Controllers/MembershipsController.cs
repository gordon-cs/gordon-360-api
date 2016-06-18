using System.Collections.Generic;
using System;
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
        [HttpGetAttribute]
        public IEnumerable<Membership> GetAll()
        {
            return Memberships.GetAll();
        }

        [HttpGetAttribute("{id}", Name="membership")]
        public IActionResult GetById(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(id);
            }
             if (id == null)
            {
                return BadRequest(id);
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (mbrshp == null)
            {
                return BadRequest();
            }
            // Set the membership_id before inserting
            mbrshp.membership_id = Guid.NewGuid().ToString();
            Memberships.Add(mbrshp);
            return CreatedAtRoute("membership", new { controller = "memberships", id = mbrshp.membership_id}, mbrshp);
        }

        [HttpPutAttribute("{id}")]
        public IActionResult Update(string id, [FromBodyAttribute] Membership mbrshp)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
        public IActionResult Delete(string id)
        {
            if(!ModelState.IsValid || id == null)
            {
                return BadRequest(id);
            }
            var result = Memberships.Remove(id);
            Memberships.Remove(id);

            if (result == null)
            {
                return NotFound(id);
            }
            else
            {
                return new NoContentResult();
            }
        }
    }
}