using Gordon360.Authorization;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    public class SportsController : GordonControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }
        /// <summary>
        /// Fetches all Sports in the RecIM database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<SportViewModel>> GetSports()
        {
            var res = _sportService.GetSports();
            return Ok(res);
        }
        /// <summary>
        /// Gets specific sport by ID
        /// </summary>
        /// <param name="sportID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{sportID}")]
        public ActionResult<SportViewModel> GetSportByID(int sportID)
        {
            var res = _sportService.GetSportByID(sportID);
            return Ok(res);
        }
        /// <summary>
        /// Update Sport in the database by ID
        /// </summary>
        /// <param name="sportID"></param>
        /// <param name="updatedSport"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{sportID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_SPORT)]
        public async Task<ActionResult<SportViewModel>> UpdateSport(int sportID, SportPatchViewModel updatedSport)
        {
            var sport = await _sportService.UpdateSportAsync(sportID,updatedSport);
            return CreatedAtAction(nameof(GetSportByID), new { sportID = sport.ID }, sport);
        }

        /// <summary>
        /// Deletes Sport in the database by ID
        /// </summary>
        /// <param name="sportID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{sportID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_SPORT)]
        public async Task<ActionResult<SportViewModel>> DeleteSport(int sportID)
        {
            return Ok();
        }
        /// <summary>
        /// Creates new Sport for RecIM
        /// </summary>
        /// <param name="newSport"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SPORT)]
        public async Task<ActionResult<SportViewModel>> CreateSport(SportUploadViewModel newSport)
        {
            var sport = await _sportService.PostSportAsync(newSport);
            return CreatedAtAction(nameof(GetSportByID), new { sportID = sport.ID }, sport);
        }

    }
}
