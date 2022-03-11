using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.ApiControllers
{
    [Route("api/[controller]")]
    public class AdvancedSearchController : GordonControllerBase
    {
        public AdvancedSearchController()
        {
        }

        /// <summary>
        /// Return a list majors.
        /// </summary>
        /// <returns> All majors</returns>
        [HttpGet]
        [Route("majors")]
        public ActionResult<IEnumerable<string>> GetMajors()
        {
            IEnumerable<string> majors = Helpers.GetMajors();
            // Return all of the majors
            return Ok(majors);
        }

        /// <summary>
        /// Return a list minors.
        /// </summary>
        /// <returns> All minors</returns>
        [HttpGet]
        [Route("minors")]
        public ActionResult<IEnumerable<string>> GetMinors()
        {
            IEnumerable<string> minors = Helpers.GetMinors();
            // Return all of the minors
            return Ok(minors);
        }

        /// <summary>
        /// Return a list minors.
        /// </summary>
        /// <returns> All minors</returns>
        [HttpGet]
        [Route("halls")]
        public ActionResult<IEnumerable<string>> GetHalls()
        {
            IEnumerable<string> halls = Helpers.GetHalls();
            // Return all of the halls
            return Ok(halls);
        }

        /// <summary>
        /// Return a list states.
        /// </summary>
        /// <returns> All states</returns>
        [HttpGet]
        [Route("states")]
        public ActionResult<IEnumerable<string>> GetStates()
        {
            IEnumerable<string> states = Helpers.GetStates();
            // Return all of the states
            return Ok(states);
        }


        /// <summary>
        /// Return a list countries.
        /// </summary>
        /// <returns> All countries</returns>
        [HttpGet]
        [Route("countries")]
        public ActionResult<IEnumerable<string>> GetCountries()
        {
            IEnumerable<string> countries = Helpers.GetCountries();
            // Return all of the countries
            return Ok(countries);
        }

        /// <summary>
        /// Return a list departments.
        /// </summary>
        /// <returns> All departments</returns>
        [HttpGet]
        [Route("departments")]
        public ActionResult<IEnumerable<string>> GetDepartments()
        {
            IEnumerable<string> departments = Helpers.GetDepartments();
            // Return all of the departments
            return Ok(departments);
        }


        /// <summary>
        /// Return a list buildings.
        /// </summary>
        /// <returns> All buildings</returns>
        [HttpGet]
        [Route("buildings")]
        public ActionResult<IEnumerable<string>> GetBuildings()
        {
            IEnumerable<string> buildings = Helpers.GetBuildings();
            // Return all of the buildings
            return Ok(buildings);
        }

    }
}
