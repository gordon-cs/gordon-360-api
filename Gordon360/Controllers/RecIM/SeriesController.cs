﻿using Gordon360.Models.CCT;
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

        /// <summary>
        /// Queries all Series with an optional active tag
        /// </summary>
        /// <param name="active"></param>
        /// <returns>Enumerable Set of Series</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<SeriesViewModel>> GetSeries([FromQuery] bool active)
        {
            var result = _seriesService.GetSeries(active);
            return Ok(result);
        }

        /// <summary>
        /// Gets specific Series
        /// </summary>
        /// <param name="seriesID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{seriesID}")]
        public ActionResult<SeriesViewModel> GetSeriesByID(int seriesID)
        {
            var result = _seriesService.GetSeriesByID(seriesID);
            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateSeries()
        {
            await _seriesService.UpdateSeries();
            return Ok();
        }

        /// <summary>
        /// Creates Series and associated SeriesTeam Models
        /// </summary>
        /// <param name="s">CreateSeriesViewModel</param>
        /// <param name="referenceSeriesID">ID of Series, used to select specific Teams </param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateSeries(CreateSeriesViewModel s, [FromQuery]int? referenceSeriesID)
        {
            int seriesID = await _seriesService.PostSeries(s, referenceSeriesID);
            return Ok(seriesID);
        }

    }
}