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
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/participations")]
    public class ParticipationsController : ApiController
    {
        private IParticipationService _participationService;

        public ParticipationsController()
        {
            var _unitOfWork = new UnitOfWork();
            _participationService = new ParticipationService(_unitOfWork);
        }
        public ParticipationsController(IParticipationService roleservice)
        {
            _participationService = roleservice; ;
        }

        /// <summary>Get all the roles a person may have within an activity</summary>
        /// <returns>A list of all the roles and their coresponding acronyms</returns>
        /// <remarks>Queries the database for all the roles that are valid</remarks>
        // GET: api/roles
        [HttpGet]
        [Route("")]

        public IHttpActionResult Get()
        {
            var all = _participationService.GetAll();
            return Ok(all);
        }

        /// <summary>Get a single role and the information about it</summary>
        /// <param name="id">The identifier for a single role</param>
        /// <returns>The information about the specified role</returns>
        /// <remarks>Queries the database and returns information about one particular role</remarks>
        // GET: api/PART_DEF/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(PART_DEF))]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            var result = _participationService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


    }
}
