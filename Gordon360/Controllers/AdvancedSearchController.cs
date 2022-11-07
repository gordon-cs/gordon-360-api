using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class AdvancedSearchController : GordonControllerBase
    {
        private readonly CCTContext _context;

        public AdvancedSearchController(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list majors.
        /// </summary>
        /// <returns> All majors</returns>
        [HttpGet]
        [Route("majors")]
        public ActionResult<IEnumerable<string>> GetMajors()
        {
            var majors = _context.Majors.OrderBy(m => m.MajorDescription)
                                 .Select(m => m.MajorDescription)
                                 .Distinct();
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
            var minors = _context.Student.Select(s => s.Minor1Description)
                                  .Distinct()
                                  .Where(s => s != null);
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
            var halls = _context.Student.Select(s => s.BuildingDescription)
                                  .Distinct()
                                  .Where(b => b != null)
                                  .OrderBy(b => b);
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
            var studentStates = _context.Student.Select(s => s.HomeState).AsEnumerable();
            var facStaffStates = _context.FacStaff.Select(fs => fs.HomeState).AsEnumerable();
            var alumniStates = _context.Alumni.Select(a => a.HomeState).AsEnumerable();

            var states = studentStates
                                  .Union(facStaffStates)
                                  .Union(alumniStates)
                                  .Distinct()
                                  .Where(s => s != null)
                                  .OrderBy(s => s);
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
            var studentCountries = _context.Student.Select(s => s.Country).AsEnumerable();
            var facstaffCountries = _context.FacStaff.Select(fs => fs.Country).AsEnumerable();
            var alumniCountries = _context.Alumni.Select(a => a.Country).AsEnumerable();

            var countries = studentCountries
                                  .Union(facstaffCountries)
                                  .Union(alumniCountries)
                                  .Distinct()
                                  .Where(s => s != null)
                                  .OrderBy(s => s);
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
            var departments = _context.FacStaff.Select(fs => fs.OnCampusDepartment)
                                   .Distinct()
                                   .Where(d => d != null)
                                   .OrderBy(d => d);
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
            var buildings = _context.FacStaff.Select(fs => fs.BuildingDescription)
                                   .Distinct()
                                   .Where(d => d != null)
                                   .OrderBy(d => d);
            return Ok(buildings);
        }

    }
}
