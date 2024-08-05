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

}
