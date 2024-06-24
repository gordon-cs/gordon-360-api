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
public class PostersController(IPosterService posterService) : ControllerBase
{
    /// <summary>
    /// Gets available posters
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPosters()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all non-expired posters
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("current")]
    public ActionResult<IEnumerable<PosterViewModel>> GetCurrentPosters()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get personalized posters based on student's activities
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("{username}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPersonalizedPosters(string username)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get posters by activity code
    /// </summary>
    /// <param name="activityCode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("activity/{activityCode}")]
    public ActionResult<IEnumerable<PosterViewModel>> GetActivityPosters(string activityCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all poster statuses
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    [Route("lookup")]
    public ActionResult<IEnumerable<PosterViewModel>> GetPosterStatuses()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates specified poster by ID
    /// </summary>
    /// <param name="posterID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPatch]
    [Route("{posterID}")]
    public ActionResult<IEnumerable<PosterViewModel>> UpdatePoster(int posterID)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Delete specified poster by ID
    /// </summary>
    /// <param name="posterID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete]
    [Route("{posterID}")]
    public ActionResult<IEnumerable<PosterViewModel>> DeletePoster(int posterID)
    {
        throw new NotImplementedException();
    }


}
