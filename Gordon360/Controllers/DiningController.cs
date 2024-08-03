using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class DiningController(CCTContext context, IDiningService diningService, IAccountService accountService) : GordonControllerBase
{
    private const string FACSTAFF_MEALPLAN_ID = "7295";

    /// <summary>
    ///  Gets information about student's dining plan and balance
    /// </summary>
    /// <returns>A DiningInfo object</returns>
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<string>> GetAsync()
    {
        var sessionCode = Helpers.GetCurrentSession(context);
        var authenticatedUsername = AuthUtils.GetUsername(User);
        var authenticatedUserId = int.Parse(accountService.GetAccountByUsername(authenticatedUsername).GordonID);
        var diningInfo = await diningService.GetDiningPlanInfoAsync(authenticatedUserId, sessionCode);

        if (diningInfo == null)
        {
            return NotFound();
        }
        if (diningInfo.ChoiceDescription == "None")
        {
            var diningBalance = await diningService.GetBalanceAsync(authenticatedUserId, FACSTAFF_MEALPLAN_ID);
            if (diningBalance == null)
            {
                return NotFound();
            }
            return Ok(diningBalance);
        }
        else
        {
            return Ok(diningInfo);
        }

    }
}
