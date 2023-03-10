﻿using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Gordon360.Models.ViewModels;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
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
        public ActionResult<IEnumerable<SeriesExtendedViewModel>> GetSeries([FromQuery] bool active)
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
        public ActionResult<SeriesExtendedViewModel> GetSeriesByID(int seriesID)
        {
            var result = _seriesService.GetSeriesByID(seriesID);
            return Ok(result);
        }


        /// <summary>
        /// Returns all types/statuses of a series available for selection
        /// </summary>
        /// <param name="type">specific series type</param>
        /// <returns></returns>
        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetSeriesTypes(string type)
        {
            var res = _seriesService.GetSeriesLookup(type);
            if (res is not null)
            {
                return Ok(res);
            }
            return NotFound();
        }

        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="seriesID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{seriesID}/schedule")]
        public ActionResult<SeriesScheduleViewModel> GetSeriesSchedule(int seriesID)
        {
            var res = _seriesService.GetSeriesScheduleByID(seriesID);
            if (res is null) return BadRequest();
            return Ok(res);
        }


        /// <summary>
        /// Updates Series Information
        /// </summary>
        /// <param name="seriesID"></param>
        /// <param name="updatedSeries"></param>
        /// <returns>modified series</returns>
        [HttpPatch]
        [Route("{seriesID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_SERIES)]
        public async Task<ActionResult<SeriesViewModel>> UpdateSeries(int seriesID, SeriesPatchViewModel updatedSeries)
        {
            var series = await _seriesService.UpdateSeriesAsync(seriesID, updatedSeries);
            return CreatedAtAction(nameof(UpdateSeries), new { seriesID = series.ID }, series);
        }

        /// <summary>
        /// Creates Series and associated SeriesTeam Models
        /// </summary>
        /// <param name="newSeries">CreateSeriesViewModel</param>
        /// <param name="referenceSeriesID">ID of Series, used to select specific Teams </param>
        /// <returns>created series</returns>
        [HttpPost]
        [Route("")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
        public async Task<ActionResult<SeriesViewModel>> CreateSeries(SeriesUploadViewModel newSeries, [FromQuery]int? referenceSeriesID)
        {
            var series = await _seriesService.PostSeriesAsync(newSeries, referenceSeriesID);
            return CreatedAtAction(nameof(CreateSeries), new { seriesID = series.ID }, series);
        }


        /// <summary>
        /// Creates schedule or finds existing schedule
        /// </summary>
        /// <param name="seriesSchedule">created schedule for series</param>
        /// <returns>created series schedule</returns>
        [HttpPut]
        [Route("schedule")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
        public async Task<ActionResult<SeriesScheduleViewModel>> CreateSeriesSchedule(SeriesScheduleUploadViewModel seriesSchedule)
        {
            var schedule = await _seriesService.PutSeriesScheduleAsync(seriesSchedule);
            return CreatedAtAction(nameof(CreateSeriesSchedule), new { scheduleID = schedule.ID }, schedule);

        }

        /// <summary>
        /// Cascade deletes all DBobjects related to given Series ID
        /// </summary>
        /// <param name="seriesID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{seriesID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_SERIES)]
        public async Task<ActionResult> DeleteSeriesCascade(int seriesID)
        {
            var res = await _seriesService.DeleteSeriesCascadeAsync(seriesID);
            return Ok(res);
        }


        /// <summary>
        /// Automatically creates Matches based on given Series
        /// </summary>
        /// <param name="seriesID"></param>
        /// <param name="request"></param>
        [HttpPost]
        [Route("{seriesID}/autoschedule")]
        //[StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
        public async Task<ActionResult<IEnumerable<MatchViewModel>>> ScheduleMatches(int seriesID, UploadScheduleRequest request)
        {
            var createdMatches = await _seriesService.ScheduleMatchesAsync(seriesID, request);
            if (createdMatches is null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(ScheduleMatches), createdMatches);
        }
    }
}
