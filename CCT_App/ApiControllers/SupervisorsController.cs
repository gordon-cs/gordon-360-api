using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CCT_App.Models;
using CCT_App.Repositories;
using CCT_App.Services;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/supervisors")]
    public class SupervisorsController : ApiController
    {
        private ISupervisorService _supervisorService;

        public SupervisorsController()
        {
            var _unitOfWork = new UnitOfWork();
            _supervisorService = new SupervisorService(_unitOfWork); 
        }
        public SupervisorsController(ISupervisorService supervisorService)
        {
            _supervisorService = supervisorService;
        }
        // GET: api/Supervisors
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _supervisorService.GetAll();
            return Ok(all);
        }

        // GET: api/Supervisors/5
        [ResponseType(typeof(IHttpActionResult))]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = _supervisorService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/Supervisors/5
        [ResponseType(typeof(IHttpActionResult))]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] SUPERVISOR supervisor)
        {
            if (!ModelState.IsValid || supervisor == null || id != supervisor.SUP_ID)
            {
                return BadRequest();
            }

            var result = _supervisorService.Update(id, supervisor);
            
            if (result == null)
            {
                return NotFound();
            }
           
            return Ok(result);
        }

        // POST: api/Supervisors
        [ResponseType(typeof(IHttpActionResult))]
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(SUPERVISOR supervisor)
        {
            if (!ModelState.IsValid || supervisor == null)
            {
                return BadRequest();
            }

            var result = _supervisorService.Add(supervisor);

            if (result == null )
            {
                return NotFound();
            }

            return Created("DefaultApi",  supervisor);
        }

        // DELETE: api/Supervisors/5
        [ResponseType(typeof(SUPERVISOR))]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {

            var result = _supervisorService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}