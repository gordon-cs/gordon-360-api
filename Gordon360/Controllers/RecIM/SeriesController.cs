﻿using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
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
        private readonly IParticipantService _participantService;

        public SeriesController(ISeriesService seriesService, IParticipantService participantService)
        {
            _seriesService = seriesService;
            _participantService = participantService;
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

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetSeriesTypes(string type)
        {
            if ( type == "series" )
            {

            }
            if ( type == "status" )
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seriesID"></param>
        /// <param name="updatedSeries"></param>
        /// <returns>modified series</returns>
        [HttpPatch]
        [Route("{seriesID}")]
        public async Task<ActionResult> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel updatedSeries)
        {
            var username = AuthUtils.GetUsername(User);
            var isAdmin = _participantService.IsAdmin(username);
            if (isAdmin)
            {
                var series = await _seriesService.UpdateSeriesAsync(seriesID, updatedSeries);
                return CreatedAtAction("UpdateSeries", series);
            }
            return Forbid();
        }

        /// <summary>
        /// Creates Series and associated SeriesTeam Models
        /// </summary>
        /// <param name="newSeries">CreateSeriesViewModel</param>
        /// <param name="referenceSeriesID">ID of Series, used to select specific Teams </param>
        /// <returns>created series</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateSeries(SeriesUploadViewModel newSeries, [FromQuery]int? referenceSeriesID)
        {
            var username = AuthUtils.GetUsername(User);
            var isAdmin = _participantService.IsAdmin(username);
            if (isAdmin)
            {
                var series = await _seriesService.PostSeriesAsync(newSeries, referenceSeriesID);
                return CreatedAtAction("CreateSeries", series);
            }
            return Forbid();
        }

    }
}
