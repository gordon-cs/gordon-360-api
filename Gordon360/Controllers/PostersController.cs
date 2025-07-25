using Gordon360.Authorization;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services;
using Gordon360.Services.RecIM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers.Api;

[Route("api/[controller]")]
public class PostersController(IPosterService posterService) : GordonControllerBase
{
    /// <summary>
    /// Gets all posters that havent been deleted
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("all")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPosters()
    {
        var res = posterService.GetPosters();
        return Ok(res);
    }

    /// <summary>
    /// Gets all non-expired posters
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("")]
    public ActionResult<IEnumerable<PosterViewModel>> GetCurrentPosters()
    {
        var res = posterService.GetCurrentPosters();
        return Ok(res);
    }

    /// <summary>
    /// Gets all non-expired posters by activity code
    /// </summary>
    [HttpGet]
    [Route("activity/{activityCode}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetCurrentPostersByActivityCode(string activityCode)
    {
       var res = posterService.GetCurrentPostersByActivityCode(activityCode);
       return Ok(res);
    }

    /// <summary>
    /// Get all non-deleted personalized posters based on student's activities
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("all/{username}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPersonalizedPosters(string username)
    {
        var res = posterService.GetPersonalizedPostersByUsername(username);
        return Ok(res);
    }

    /// <summary>
    /// Get all not-deleted posters by activity code
    /// </summary>
    /// <param name="activityCode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("all/activity/{activityCode}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetActivityPosters(string activityCode)
    {
        var res = posterService.GetPostersByActivityCode(activityCode);
        return Ok(res);
    }

    /// <summary>
    /// Gets all poster statuses
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("all/lookup")]
    public ActionResult<IEnumerable<string>> GetPosterStatuses()
    {
        var res = posterService.GetPosterStatuses();
        return Ok(res);
    }

    /// <summary>
    /// Gets poster object by ID
    /// </summary>
    /// <returns>
    /// Poster object
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("{posterID}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPosterByID(int posterID)
    {
        var res = posterService.GetPosterByID(posterID);
        return Ok(res);
    }

    /// <summary>
    /// Creates a poster
    /// </summary>
    /// <param name="poster"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost]
    [Route("")]
    public async Task<ActionResult<PosterViewModel>> CreatePoster(PosterUploadViewModel poster)
    {
        var newPoster = await posterService.PostPosterAsync(poster);
        return CreatedAtAction(nameof(GetPosterByID), new { posterID = newPoster.ID }, poster);
    }

    /// <summary>
    /// Updates specified poster by ID
    /// </summary>
    /// <param name="posterID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPatch]
    [Route("{posterID}")]
    public async Task<ActionResult<PosterViewModel>> UpdatePoster(int posterID, [FromBody] PosterPatchViewModel updatedPoster)
    {
        var poster = await posterService.UpdatePosterAsync(posterID, updatedPoster);
        return CreatedAtAction(nameof(GetPosterByID), new { posterID = poster.ID }, poster);
    }

    /// <summary>
    /// Delete specified poster by ID
    /// </summary>
    /// <param name="posterID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete]
    [Route("{posterID}")]
    public async Task<ActionResult<PosterViewModel>> DeletePoster(int posterID)
    {
        var res = await posterService.DeletePosterAsync(posterID);
        return Ok(res);
    }

    /// <summary>
    /// Changes ExpirationDate to the current moment to effectively 
    ///"hide" specific poster from view
    ///</summary>
    ///<returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("hide/{posterID}")]
    public async Task<IActionResult> HidePoster(int posterID)
    {
        try
        {
            var result = await posterService.HidePosterAsync(posterID);
            return Ok(result);
        }
        catch (ResourceNotFoundException e)
        {
            return NotFound(e.ExceptionMessage);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }


}
