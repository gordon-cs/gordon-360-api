using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

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
    public ActionResult<SeriesScheduleExtendedViewModel> GetSeriesScheduleByID(int seriesID)
    {
        var res = _seriesService.GetSeriesScheduleByID(seriesID);
        if (res is null) return NotFound();
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
    public async Task<ActionResult<SeriesViewModel>> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel updatedSeries)
    {
        var series = await _seriesService.UpdateSeriesAsync(seriesID, updatedSeries);
        return CreatedAtAction(nameof(GetSeriesByID), new { seriesID = series.ID }, series);
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
    public async Task<ActionResult<SeriesViewModel>> CreateSeriesAsync(SeriesUploadViewModel newSeries, [FromQuery]int? referenceSeriesID)
    {
        var series = await _seriesService.PostSeriesAsync(newSeries, referenceSeriesID);
        return CreatedAtAction(nameof(GetSeriesByID), new { seriesID = series.ID }, series);
    }


    /// <summary>
    /// Creates schedule or finds existing schedule
    /// </summary>
    /// <param name="seriesSchedule">created schedule for series</param>
    /// <returns>created series schedule</returns>
    [HttpPut]
    [Route("schedule")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult<SeriesScheduleViewModel>> CreateSeriesScheduleAsync(SeriesScheduleUploadViewModel seriesSchedule)
    {
        var schedule = await _seriesService.PutSeriesScheduleAsync(seriesSchedule);
        return Ok(schedule);

    }

    /// <summary>
    /// Updates team record manually (mitigates potential bugs)
    /// </summary>
    /// <param name="seriesID"></param>
    /// <param name="update">Updated team record</param>
    /// <returns>updated team record</returns>
    [HttpPatch]
    [Route("{seriesID}/teamrecord")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult<TeamRecordViewModel>> UpdateSeriesTeamRecordAsync(int seriesID, TeamRecordPatchViewModel update)
    {
        var record = await _seriesService.UpdateSeriesTeamRecordAsync(seriesID ,update);
        return Ok(record);
    }

    /// <summary>
    /// gets all series winners
    /// </summary>
    /// <param name="seriesID"></param>
    [HttpGet]
    [Route("{seriesID}/winners")]
    public ActionResult<IEnumerable<AffiliationPointsViewModel>> GetSeriesWinners(int seriesID)
    {
        var res = _seriesService.GetSeriesWinners(seriesID);
        return Ok(res);
    }

    /// <summary>
    /// Adds/Removes additional winners to a series
    /// </summary>
    /// <param name="seriesID"></param>
    /// <param name="vm"></param>
    [HttpPut]
    [Route("{seriesID}/winners")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult> UpdateSeriesWinnersAsync(int seriesID, AffiliationPointsUploadViewModel vm)
    {
        await _seriesService.HandleAdditionalSeriesWinnersAsync(vm);
        return Ok();
    }

    /// <summary>
    /// Cascade deletes all DBobjects related to given Series ID
    /// </summary>
    /// <param name="seriesID"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{seriesID}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult> DeleteSeriesCascadeAsync(int seriesID)
    {
        var res = await _seriesService.DeleteSeriesCascadeAsync(seriesID);
        return Ok(res);
    }


    /// <summary>
    /// Automatically creates Matches based on given Series
    /// </summary>
    /// <param name="seriesID"></param>
    /// <param name="request">optional request data, used for additional options on autoscheduling</param>
    [HttpPost]
    [Route("{seriesID}/autoschedule")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult<IEnumerable<MatchViewModel>>> ScheduleMatchesAsync(int seriesID, UploadScheduleRequest? request)
    {
        var req = request ?? new UploadScheduleRequest();
        var createdMatches = await _seriesService.ScheduleMatchesAsync(seriesID, req);
        if (createdMatches is null)
        {
            return BadRequest();
        }
        return Ok(createdMatches);
    }

    /// <summary>
    /// Gives last date and number of matches of which the Auto Scheduler will create matches until.
    /// </summary>
    /// <param name="seriesID"></param>
    /// <param name="RoundRobinMatchCapacity"></param>
    /// <param name="NumberOfLadderMatches"></param>
    [HttpGet]
    [Route("{seriesID}/autoschedule")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SERIES)]
    public async Task<ActionResult<SeriesAutoSchedulerEstimateViewModel>> GetAutoScheduleEstimate(
        int seriesID, [FromQuery] int RoundRobinMatchCapacity,  [FromQuery] int NumberOfLadderMatches)
    {
        var req = new UploadScheduleRequest()
        {
            RoundRobinMatchCapacity = RoundRobinMatchCapacity,
            NumberOfLadderMatches = NumberOfLadderMatches,
        };

        var estimate = _seriesService.GetScheduleMatchesEstimateAsync(seriesID, req);
        if (estimate is null)
        {
            return BadRequest();
        }
        return Ok(estimate);
    }

    /// <summary>
    /// Gets available bracket information for a givaen series
    /// </summary>
    /// <param name="seriesID"></param>
    [HttpGet]
    [Route("{seriesID}/bracket")]
    public ActionResult<IEnumerable<MatchBracketViewModel>> GetBracket(int seriesID)
    {
        var res = _seriesService.GetSeriesBracketInformation(seriesID);
        return Ok(res);
    }
}
