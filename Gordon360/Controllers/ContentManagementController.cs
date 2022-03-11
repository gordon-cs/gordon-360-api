using Gordon360.AuthorizationFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class ContentManagementController : GordonControllerBase
    {
        private readonly IContentManagementService _contentManagementService;

        public ContentManagementController(IContentManagementService contentManagementService)
        {
            _contentManagementService = contentManagementService;
        }

        /// <summary>Get all the slider content for the dashboard slider</summary>
        /// <returns>A list of all the slides for the slider</returns>
        /// <remarks>Queries the database for all entries in slider table</remarks>
        // GET: api/cms/slider
        [HttpGet]
        [Route("slider")]
        [AllowAnonymous]
        [StateYourBusiness(operation = Operation.READ_PUBLIC, resource = Resource.SLIDER)]
        public ActionResult<SliderViewModel> GetSliderContent()
        {
            var result = _contentManagementService.DEPRECATED_GetSliderContent();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Get all the banner slides for the dashboard banner</summary>
        /// <returns>A list of all the slides for the banner</returns>
        [HttpGet]
        [Route("banner")]
        [StateYourBusiness(operation = Operation.READ_PUBLIC, resource = Resource.SLIDER)]
        public IHttpActionResult GetBannerSlides()
        {
            var result = _contentManagementService.GetBannerSlides();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Post a new slide for the dashboard banner</summary>
        /// <returns>The posted banner</returns>
        [HttpPost]
        [Route("banner")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.SLIDER)]
        public IHttpActionResult PostBannerSlide(BannerSlidePostViewModel slide)
        {
            var serverURL = Url.Content("~/");
            var result = _contentManagementService.AddBannerSlide(slide, serverURL);
            if (result == null)
            {
                return NotFound();
            }

            return Created($"api/cms/banner/{result.ID}", result);
        }

        /// <summary>Remove a slide from the dashboard banner</summary>
        /// <returns>ID of the slide to remove</returns>
        [HttpDelete]
        [Route("banner/{slideID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.SLIDER)]
        public IHttpActionResult DeleteBannerSlide(int slideID)
        {
            var result = _contentManagementService.DeleteBannerSlide(slideID);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
