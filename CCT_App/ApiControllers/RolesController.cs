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
using CCT_App.Services;
using CCT_App.Repositories;

namespace CCT_App.Controllers.Api
{
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private IRoleService _roleService;

        public RolesController()
        {
            var _unitOfWork = new UnitOfWork();
            _roleService = new RoleService(_unitOfWork);
        }
        public RolesController(IRoleService roleservice)
        {
            _roleService = roleservice; ;
        }
        // GET: api/roles
        [HttpGet]
        [Route("")]

        public IHttpActionResult Get()
        {
            var all = _roleService.GetAll();
            return Ok(all);
        }

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
            var result = _roleService.Get(id);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        
    }
}