using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class VictoryPromiseController(IVictoryPromiseService victoryPromiseService) : ControllerBase
{

    /// <summary>
    ///  Gets current victory promise scores
    /// </summary>
    /// <returns>A VP Json</returns>
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<VictoryPromiseViewModel>>> Get()
    {
        var username = AuthUtils.GetUsername(User);

        var result = await victoryPromiseService.GetVPScoresAsync(username);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
