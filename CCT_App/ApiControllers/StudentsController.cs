﻿using System;
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
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        private IStudentService _studentService;

        public StudentsController()
        {
            var _unitOfWork = new UnitOfWork();
            _studentService = new StudentService(_unitOfWork);
        }
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Students
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult Get()
        {
            var all = _studentService.GetAll();
            return Ok(all);
        }

        // GET: api/Students/5
        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult Get(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var result = _studentService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        
        [ResponseType(typeof(IHttpActionResult))]
        [Route("{id}/memberships")]
        [HttpGet]
        public IHttpActionResult GetActivitiesForStudent(string id)
        {
            if(!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            var result = _studentService.GetActivitiesForStudent(id);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);

        }

        
  
    }
}