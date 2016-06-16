using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using cct_api.models;

namespace cct_api.controllers
{
    [Route("api/[controller]")]
    public class ActivitiesController : Controller
    {
        public IActivityRepository Activities;
        public ActivitiesController(IActivityRepository activities)
        {
            Activities = activities;
        }
        [RouteAttribute("")]
        public IEnumerable<Activity> GetAll()
        {
            return Activities.GetAll();
        }

        [HttpGetAttribute("{id}")]
        public IActionResult GetById(string id)
        {
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
            if (activ == null)
            {
                return BadRequest();
            }
            Activities.Add(activ);
            return CreatedAtRoute("GetActivity", new { controller = "Activity", id = activ.activity_id}, activ);
        }

        [HttpPutAttribute("{id:int}")]
        public IActionResult Update(string id, [FromBodyAttribute] Activity activ)
        {
            if (activ == null || activ.activity_id != id)
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
    }
}