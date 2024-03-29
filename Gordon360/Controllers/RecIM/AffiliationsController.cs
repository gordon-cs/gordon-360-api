﻿using Gordon360.Authorization;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

[Route("api/recim/[controller]")]
public class AffiliationsController(IAffiliationService affiliationService) : GordonControllerBase
{

    /// <summary>
    /// Gets all stored affiliations/halls/clubs with associated
    /// Points/Activities Won : Winning Team
    /// </summary>
    [HttpGet]
    [Route("")]
    public ActionResult<IEnumerable<AffiliationExtendedViewModel>> GetAllAffiliationDetails()
    {
        return Ok(affiliationService.GetAllAffiliationDetails());
    }

    /// <summary>
    /// Gets specific stored affiliation with associated
    /// Points/Activities Won : Winning Team
    /// </summary>
    [HttpGet]
    [Route("{affiliationName}")]
    public ActionResult<IEnumerable<AffiliationExtendedViewModel>> GetAffiliationDetailsByName(string affiliationName)
    {
        return Ok(affiliationService.GetAffiliationDetailsByName(affiliationName));
    }

    /// <summary>
    /// Creates new Affiliation
    /// </summary>
    /// <param name="affiliationName"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_AFFILIATION)]
    public async Task<ActionResult> CreateAffiliation([FromBody] string affiliationName)
    {
        var res = await affiliationService.CreateAffiliation(affiliationName);
        return CreatedAtAction(nameof(GetAffiliationDetailsByName), new { affiliationName = res }, res);
    }

    /// <summary>
    /// Updates an affiliation's logo and/or name
    /// </summary>
    /// <param name="affiliationName"></param>
    /// <param name="update">updated instance of affiliation</param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{affiliationName}")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_AFFILIATION)]
    public async Task<ActionResult> UpdateAffiliation(string affiliationName, AffiliationPatchViewModel update)
    {
        var res = await affiliationService.UpdateAffiliationAsync(affiliationName, update);
        return CreatedAtAction(nameof(GetAffiliationDetailsByName), new { affiliationName = res }, res);
    }

    /// <summary>
    /// Creates new AffiliationPoints entry 
    /// (affiliationname, activity where points were attributed, optional points)
    /// </summary>
    /// <param name="vm">put viewmodel</param>
    /// <param name="affiliationName"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{affiliationName}/points")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_AFFILIATION)]
    public async Task<ActionResult> AddPointsToAffilliation(string affiliationName, AffiliationPointsUploadViewModel vm)
    {
        var res = await affiliationService.AddPointsToAffilliationAsync(affiliationName, vm);
        return CreatedAtAction(nameof(GetAffiliationDetailsByName), new { affiliationName = res }, res);
    }

    /// <summary>
    /// Deletes affiliation
    /// </summary>
    /// <param name="affiliationName"></param>
    [HttpDelete]
    [Route("")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_AFFILIATION)]
    public async Task<ActionResult> DeleteAffiliation(string affiliationName)
    {
        await affiliationService.DeleteAffiliationAsync(affiliationName);
        return Ok();
    }
}
