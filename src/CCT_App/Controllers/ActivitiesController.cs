using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using cct_api.models;

namespace cct_api.controllers
{
    // All urls of the form "api/controllername" get routed to this class.
    [Route("api/[controller]" )]
    public class ActivitiesController : Controller
    {
        public IActivityRepository Activities;
        // Constructor for the controller
        public ActivitiesController(IActivityRepository activities)
        {
            Activities = activities;
        }
        // A url of the form GET /api/activities gets routed here.
        [HttpGetAttribute]
        public IEnumerable<Activity> GetAll()
        {
            return Activities.GetAll();
        }
        // A url of the form GET /api/activities/:id gets routed here.
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

        // A post request to api/activies gets routed here. Post request must have content-type of application/json
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

        // Put requests to api/activities/:id get routed here.
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

        // Delete requests to api/activitites/:id get routed here.
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