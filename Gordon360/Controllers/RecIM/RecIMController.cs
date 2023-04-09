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
    [Route("api/recim/admin")]
    public class RecIMAdminController : GordonControllerBase
    {
        private readonly IRecIMService _recimService;
        public RecIMAdminController(IRecIMService recimService)
        {
            _recimService = recimService;
        }

        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.RECIM)]
        public ActionResult<RecIMGeneralReportViewModel> GetReport(DateTime start, DateTime end)
        {
            var res = _recimService.GetReport(start,end);
            return Ok(res);
        }


    }
}
