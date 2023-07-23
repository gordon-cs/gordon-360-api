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

    }
}
