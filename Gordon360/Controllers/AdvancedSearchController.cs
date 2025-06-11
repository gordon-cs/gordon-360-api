using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Models.webSQL.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class AdvancedSearchController(CCTContext context) : GordonControllerBase
{

    /// <summary>
    /// Return a list majors.
    /// </summary>
    /// <returns> All majors</returns>
    [HttpGet]
    [Route("majors")]
    public ActionResult<IEnumerable<string>> GetMajors()
    {
        var majors = context.Majors.OrderBy(m => m.MajorDescription)
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
        var minors = context.Student.Select(s => s.Minor1Description)
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
        var halls = context.Student.Select(s => s.BuildingDescription)
                              .Distinct()
                              .Where(b => b != null)
                              .OrderBy(b => b);
        return Ok(halls);
    }

    /// <summary>
    /// Return a list states.
    /// </summary>
    ///
    /// <remarks>DEPRECATED: Use AddressController instead</remarks>
    /// <returns> All states</returns>
    [HttpGet]
    [Route("states")]
    public ActionResult<IEnumerable<string>> DEPRECATED_GetStates()
    {
        var studentStates = context.Student.Select(s => s.HomeState).AsEnumerable();
        var facStaffStates = context.FacStaff.Select(fs => fs.HomeState).AsEnumerable();
        var alumniStates = context.Alumni.Select(a => a.HomeState).AsEnumerable();

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
    /// 
    /// <remarks>DEPRECATED: Use AddressController instead</remarks>
    /// <returns> All countries</returns>
    [HttpGet]
    [Route("countries")]
    public ActionResult<IEnumerable<string>> DEPREACTED_GetCountries()
    {
        var studentCountries = context.Student.Select(s => s.Country).AsEnumerable();
        var facstaffCountries = context.FacStaff.Select(fs => fs.Country).AsEnumerable();
        var alumniCountries = context.Alumni.Select(a => a.Country).AsEnumerable();

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
        var departments = context.FacStaff.Select(fs => fs.OnCampusDepartment)
                               .Distinct()
                               .Where(d => d != null)
                               .OrderBy(d => d);
        return Ok(departments);
    }


    /// <summary>
    /// Return a list of buildings.
    /// </summary>
    /// <returns> All buildings</returns>
    [HttpGet]
    [Route("building")]
    public async Task<ActionResult<IEnumerable<BuildingViewModel>>> GetBuildingsAsync([FromServices] webSQLContext webSQLContext)
    {
        var buildings = await webSQLContext.Procedures.account_list_buildingsAsync();
        return Ok(buildings.Select(b => new BuildingViewModel(b.BLDG_CDE, b.BUILDING_DESC)));
    }

    /// <summary>
    /// Return a list of buildings.
    /// </summary>
    /// <returns> All buildings</returns>
    [Obsolete("Use GetBuildingsAsync that gives structured building data")]
    [HttpGet]
    [Route("buildings")]
    public ActionResult<IEnumerable<string>> GetBuildings()
    {
        var buildings = context.FacStaff.Select(fs => fs.BuildingDescription)
                               .Distinct()
                               .Where(d => d != null)
                               .OrderBy(d => d);
        return Ok(buildings);
    }

    /// <summary>
    /// Return a list of involvements' descriptions.
    /// </summary>
    /// <returns> All involvements</returns>
    [HttpGet]
    [Route("involvements")]
    public ActionResult<IEnumerable<string>> GetInvolvements()
    {
        var involvements = context.MembershipView.Select(m => m.ActivityDescription)
                               .Distinct()
                               .Where(d => d != null)
                               .OrderBy(d => d);
        return Ok(involvements);
    }

    /// <summary>
    /// Return a list of genders excluding unknown.
    /// </summary>
    /// <returns> All genders</returns>
    [HttpGet]
    [Route("gender")]
    public ActionResult<IEnumerable<string>> GetGender()
    {
        var studentGender = context.Student.Select(s => s.Gender);
        var facstaffGender = context.FacStaff.Select(fs => fs.Gender);
        var alumniGender = context.Alumni.Select(a => a.Gender);
        var genderCodes = studentGender
                               .Union(facstaffGender)
                               .Union(alumniGender)
                               .Where(s => s != null && s != "U")
                               .Distinct();
        var genderList = genderCodes
                               .Select(g => new { 
                                       value = g,
                                       label = g == "F" ? "Female" : g == "M" ? "Male" : g
                               })
                               .OrderBy(g => g.label);
        return Ok(genderList);
    }
}
