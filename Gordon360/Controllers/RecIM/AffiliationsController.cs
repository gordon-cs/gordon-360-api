using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    public class AffiliationsController : GordonControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IAffiliationService _affiliationService;

        public AffiliationsController(IActivityService activityService, IAffiliationService affiliationService)
        {
            _activityService = activityService;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets specific stored affiliation with associated
        /// Points/Activities Won : Winning Team
        /// </summary>
        [HttpGet]
        [Route("{affiliationName}")]
        public ActionResult<IEnumerable<AffiliationExtendedViewModel>> GetAffiliationDetailsByName([FromBody] string affiliationName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates new Affiliation
        /// </summary>
        /// <param name="affiliationName"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> PutAffiliation(string affiliationName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates new AffiliationPoints entry 
        /// (affiliationname, activity where points were attributed, optional points)
        /// </summary>
        /// <param name="affiliationName"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{affiliationName}")]
        public async Task<ActionResult> AddPointsToAffilliation(AffiliationPointsUpdateViewModel affiliationName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes affiliation
        /// </summary>
        /// <param name="affiliationName"></param>
        [HttpDelete]
        [Route("")]
        public async Task<ActionResult> DeleteAffiliation(string affiliationName)
        {
            throw new NotImplementedException();
        }
    }
}
