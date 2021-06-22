using System;
using System.Collections.Generic;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [Route("api/advanced-search")]
    public class AdvancedSearchController : ControllerBase
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
        public ActionResult<IEnumerable<String>> GetMajors()
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
        public ActionResult<IEnumerable<String>> GetMinors()
        {
            IEnumerable<String> minors = Helpers.GetMinors();
            // Return all of the minors
            return Ok(minors);
        }

        /// <summary>
        /// Return a list minors.
        /// </summary>
        /// <returns> All minors</returns>
        [HttpGet]
        [Route("halls")]
        public ActionResult<IEnumerable<String>> GetHalls()
        {
            IEnumerable<String> halls = Helpers.GetHalls();
            // Return all of the halls
            return Ok(halls);
        }

        /// <summary>
        /// Return a list states.
        /// </summary>
        /// <returns> All states</returns>
        [HttpGet]
        [Route("states")]
        public ActionResult<IEnumerable<String>> GetStates()
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
        public ActionResult<IEnumerable<String>> GetCountries()
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
        public ActionResult<IEnumerable<String>> GetDepartments()
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
        public ActionResult<IEnumerable<String>> GetBuildings()
        {
            IEnumerable<String> buildings = Helpers.GetBuildings();
            // Return all of the buildings
            return Ok(buildings);
        }

    }
}
