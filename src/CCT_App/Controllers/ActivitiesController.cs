using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using cct_api.models;

namespace cct_api.controllers
{
    [Route("api/[controller]" )]
    public class ActivitiesController : Controller
    {
        public IActivityRepository Activities;

        public ActivitiesController(IActivityRepository activities)
        {
            Activities = activities;
        }

        [HttpGetAttribute]
        public IEnumerable<Activity> GetAll()
        {
            return Activities.GetAll();
        }

        [HttpGetAttribute("{id}", Name="activity")]
        public IActionResult GetById([FromRouteAttribute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(id);
            }
            if (id == null)
            {
                return BadRequest(id);
            }
            var activity = Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }
            return new ObjectResult(activity);
        }

        [HttpPostAttribute]
        public IActionResult Create([FromBodyAttribute] Activity activ)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (activ == null)
            {
                return BadRequest();
            }
            // Set the activity_id before inserting
            activ.activity_id = Guid.NewGuid().ToString();
            Activities.Add(activ);
            return CreatedAtRoute("activity", new { controller = "activities", id = activ.activity_id}, activ);
        }

        [HttpPutAttribute("{id}")]
        public IActionResult Update(string id,  [FromBodyAttribute] Activity activ)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (activ == null || activ.activity_id != id || id == null)
            {
                return BadRequest();
            }

            var activity = Activities.Find(id);
            if ( activity == null)
            {
                return NotFound();
            }

            Activities.Update(activ);
            return new NoContentResult();
        }

        [HttpDeleteAttribute("{id}")]
        public IActionResult Delete(string id)
        {
            if(!ModelState.IsValid || id == null)
            {
                return BadRequest(id);
            }
            var result = Activities.Remove(id);
            // No activity with that id was found
            if (result == null)
            {
                return NotFound(id);
            }
            // Activity was found and deleted
            else
            {
                return new NoContentResult();
            }
        }
    }
}