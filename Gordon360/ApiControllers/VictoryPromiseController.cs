using System.Collections.Generic;
using System.Security.Claims;
using Gordon360.Database.CCT;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers.Api
{
    [Route("api/vpscore")]
    [CustomExceptionFilter]
    [Authorize]
    public class VictoryPromiseController : ControllerBase
    {
        private readonly IVictoryPromiseService _victoryPromiseService;

        public VictoryPromiseController(CCTContext context)
        {
            _victoryPromiseService = new VictoryPromiseService(context);
        }
        public VictoryPromiseController(IVictoryPromiseService victoryPromiseService)
        {
            _victoryPromiseService = victoryPromiseService;
        }

        /// <summary>
        ///  Gets current victory promise scores
        /// </summary>
        /// <returns>A VP Json</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<VictoryPromiseViewModel>> Get()
        {
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = _victoryPromiseService.GetVPScores(authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
