using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

[Route("api/recim/admin")]
public class RecIMAdminController : GordonControllerBase
{
    private readonly IRecIMService _recimService;
    public RecIMAdminController(IRecIMService recimService)
    {
        _recimService = recimService;
    }

    /// <summary>
    /// Rec-IM Reporting:
    ///     Every semester Rec-IM needs to report, their own client,
    ///     information regarding new users, retention of users, 
    ///     gender ratio, involvements...
    ///     
    ///     This route does not specify semester terms, however, is more
    ///     specific in which it can select time constraints from beginning
    ///     to end with no limit to time in between
    /// </summary>
    /// <param name="startTime">report start time</param>
    /// <param name="endTime">report end time</param>
    /// <returns></returns>
    [HttpGet]
    [Route("report")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.RECIM)]
    public ActionResult<RecIMGeneralReportViewModel> GetReport(DateTime startTime, DateTime endTime)
    {
        var res = _recimService.GetReport(startTime,endTime);
        return Ok(res);
    }


}
