using System;
using System.Collections.Generic;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Static.Methods;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/advanced-search")]
    public class AdvancedSearchController : ApiController
    {
        public AdvancedSearchController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Return a list majors.
        /// </summary>
        /// <returns> All majors</returns>
        [HttpGet]
        [Route("majors")]
        public IHttpActionResult GetMajors()
        {
            IEnumerable<String> majors = Helpers.GetMajors();
            // Return all of the majors
            return Ok(majors);
        }

        /// <summary>
        /// Return a list minors.
        /// </summary>
        /// <returns> All minors</returns>
        [HttpGet]
        [Route("minors")]
        public IHttpActionResult GetMinors()
        {
            IEnumerable<String> minors = Helpers.GetMinors();
            // Return all of the majors
            return Ok(minors);
        }

        /// <summary>
        /// Return a list states.
        /// </summary>
        /// <returns> All states</returns>
        [HttpGet]
        [Route("states")]
        public IHttpActionResult GetStates()
        {
            IEnumerable<String> states = Helpers.GetStates();
            // Return all of the states
            return Ok(states);
        }


        /// <summary>
        /// Return a list countries.
        /// </summary>
        /// <returns> All countries</returns>
        [HttpGet]
        [Route("countries")]
        public IHttpActionResult GetCountries()
        {
            IEnumerable<String> countries = Helpers.GetCountries();
            // Return all of the countries
            return Ok(countries);
        }

        /// <summary>
        /// Return a list departments.
        /// </summary>
        /// <returns> All departments</returns>
        [HttpGet]
        [Route("departments")]
        public IHttpActionResult GetDepartments()
        {
            IEnumerable<String> departments = Helpers.GetDepartments();
            // Return all of the departments
            return Ok(departments);
        }


        /// <summary>
        /// Return a list buildings.
        /// </summary>
        /// <returns> All buildings</returns>
        [HttpGet]
        [Route("buildings")]
        public IHttpActionResult GetBuildings()
        {
            IEnumerable<String> buildings = Helpers.GetBuildings();
            // Return all of the buildings
            return Ok(buildings);
        }




    }
}