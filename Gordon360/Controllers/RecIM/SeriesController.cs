using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    [AllowAnonymous]
    public class SeriesController : GordonControllerBase
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }


        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<ActivityViewModel>> GetSeries([FromQuery] bool active)
        {
            return null;
        }

        [HttpGet]
        [Route("{seriesID}")]
        public ActionResult<ActivityViewModel> GetSeriesByID(int seriesID)
        {
            return null;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateSeries()
        {
            return null;
        }

    }
}
