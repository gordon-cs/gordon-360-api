using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    public class AffiliationsController : GordonControllerBase
    {
        private readonly IAffiliationService _affiliationService;

        public AffiliationsController(IAffiliationService affiliationService)
        {
            _affiliationService = affiliationService;
        }

        /// <summary>
        /// Gets all stored affiliations/halls/clubs with associated
        /// Points/Activities Won : Winning Team
        /// </summary>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<AffiliationExtendedViewModel>> GetAllAffiliationDetails()
        {
            return Ok(_affiliationService.GetAllAffiliationDetails());
        }

        /// <summary>
        /// Gets specific stored affiliation with associated
        /// Points/Activities Won : Winning Team
        /// </summary>
        [HttpGet]
        [Route("{affiliationName}")]
        public ActionResult<IEnumerable<AffiliationExtendedViewModel>> GetAffiliationDetailsByName(string affiliationName)
        {
            return Ok(_affiliationService.GetAffiliationDetailsByName(affiliationName));
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
            var res = await _affiliationService.CreateAffiliation(affiliationName);
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
        public async Task<ActionResult> AddPointsToAffilliation(string affiliationName,AffiliationPointsUpdateViewModel vm)
        {
            var res = await _affiliationService.AddPointsToAffilliation(affiliationName, vm);
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
            await _affiliationService.DeleteAffiliation(affiliationName);
            return Ok();
        }
    }
}
