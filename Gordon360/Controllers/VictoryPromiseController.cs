﻿using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
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
            var username = AuthUtils.GetAuthenticatedUserUsername(User);

            var result = _victoryPromiseService.GetVPScores(username);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
