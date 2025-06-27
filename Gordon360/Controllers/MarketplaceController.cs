using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class MarketplaceController(IMarketplaceService marketplaceService) : GordonControllerBase
    {
        /// <summary>
        /// Get all marketplace listings.
        /// </summary>
        /// <returns>List of all marketplace listings.</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<MarketplaceListingViewModel>> GetAllListings()
        {
            var result = marketplaceService.GetAllListings();
            return Ok(result);
        }

        /// <summary>
        /// Get a specific marketplace listing by ID.
        /// </summary>
        /// <param name="listingId">The ID of the listing.</param>
        /// <returns>The marketplace listing.</returns>
        [HttpGet]
        [Route("{listingId}")]
        public ActionResult<MarketplaceListingViewModel> GetListingById(int listingId)
        {
            var result = marketplaceService.GetListingById(listingId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new marketplace listing.
        /// </summary>
        /// <param name="newListing">The details of the new listing.</param>
        /// <returns>The created listing.</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<MarketplaceListingViewModel>> CreateListing([FromBody] MarketplaceListingUploadViewModel newListing)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var result = await marketplaceService.CreateListingAsync(newListing, authenticatedUserUsername);
            return CreatedAtAction(nameof(GetListingById), new { listingId = result.Id }, result);
        }

        /// <summary>
        /// Update an existing marketplace listing.
        /// </summary>
        /// <param name="listingId">The ID of the listing to update.</param>
        /// <param name="updatedListing">The updated listing details.</param>
        /// <returns>The updated listing.</returns>
        [HttpPut]
        [Route("{listingId}")]
        public async Task<ActionResult<MarketplaceListingViewModel>> UpdateListing(int listingId, [FromBody] MarketplaceListingUploadViewModel updatedListing)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var viewerGroups = AuthUtils.GetGroups(User);
            var listing = marketplaceService.GetListingById(listingId);
            if (listing == null)
                return NotFound();

            // Only original poster or SiteAdmin can update
            if (!string.Equals(listing.PosterUsername, authenticatedUserUsername, StringComparison.OrdinalIgnoreCase)
                && !viewerGroups.Contains(AuthGroup.SiteAdmin))
            {
                return Forbid();
            }

            var result = await marketplaceService.UpdateListingAsync(listingId, updatedListing);
            return Ok(result);
        }

        /// <summary>
        /// Delete a marketplace listing.
        /// </summary>
        /// <param name="listingId">The ID of the listing to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        [HttpDelete]
        [Route("{listingId}")]
        public async Task<ActionResult> DeleteListing(int listingId)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var viewerGroups = AuthUtils.GetGroups(User);
            var listing = marketplaceService.GetListingById(listingId);
            if (listing == null)
                return NotFound();

            // Only SiteAdmin can delete
            if (!viewerGroups.Contains(AuthGroup.SiteAdmin))
            {
                return Forbid();
            }

            await marketplaceService.DeleteListingAsync(listingId);
            return Ok();
        }

        /// <summary>
        /// Change the status of a marketplace listing.
        /// </summary>
        /// <param name="listingId">The ID of the listing.</param>
        /// <param name="status">The new status.</param>
        /// <returns>The updated listing.</returns>
        [HttpPut]
        [Route("{listingId}/status")]
        public async Task<ActionResult<MarketplaceListingViewModel>> ChangeListingStatus(int listingId, [FromBody] string status)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var viewerGroups = AuthUtils.GetGroups(User);
            var listing = marketplaceService.GetListingById(listingId);
            if (listing == null)
                return NotFound();

            // Only original poster or SiteAdmin can change status
            if (!string.Equals(listing.PosterUsername, authenticatedUserUsername, StringComparison.OrdinalIgnoreCase)
                && !viewerGroups.Contains(AuthGroup.SiteAdmin))
            {
                return Forbid();
            }

            var result = await marketplaceService.ChangeListingStatusAsync(listingId, status);
            return Ok(result);
        }




        [HttpGet]
        [Route("count")]
        public IActionResult GetFilteredListingsCount(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string search = null)
        {
            var count = marketplaceService.GetFilteredListingsCount(categoryId, statusId, minPrice, maxPrice, search);
            return Ok(count);
        }



        /// <summary>
        /// Get marketplace listings filtered by category, status, and price.
        /// </summary>
        /// <param name="categoryId">Optional category ID.</param>
        /// <param name="statusId">Optional status ID.</param>
        /// <param name="minPrice">Optional minimum price.</param>
        /// <param name="maxPrice">Optional maximum price.</param>
        /// <param name="search">Optional search term.</param>
        /// <param name="sortBy">Optional sort criteria.</param>
        /// <param name="desc">Optional sort order (descending).</param>
        /// <param name="page">Optional page number for pagination.</param>
        /// <param name="pageSize">Optional page size for pagination.</param>
        /// <returns>Filtered marketplace listings.</returns>
        [HttpGet("filter")]
        public ActionResult<IEnumerable<MarketplaceListingViewModel>> GetFilteredListings(
            [FromQuery] int? categoryId,
            [FromQuery] int? statusId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string search = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool desc = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = marketplaceService.GetFilteredListings(categoryId, statusId, minPrice, maxPrice, search, sortBy, desc, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get all listings for the authenticated user.
        /// </summary>
        [HttpGet("mylistings")]
        public ActionResult<IEnumerable<MarketplaceListingViewModel>> GetMyListings()
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var result = marketplaceService.GetUserListings(authenticatedUserUsername);
            return Ok(result);
        }

        /// <summary>
        /// Get all item conditions.
        /// </summary>
        /// <returns>List of all item conditions.</returns>
        [HttpGet("conditions")]
        public ActionResult<IEnumerable<ItemCondition>> GetConditions([FromServices] CCTContext context)
        {
            var conditions = context.ItemCondition.OrderBy(c => c.Id).ToList();
            return Ok(conditions);
        }

        /// <summary>
        /// Get all item categories.
        /// </summary>
        /// <returns>List of all item categories.</returns>
        [HttpGet("categories")]
        public ActionResult<IEnumerable<ItemCategory>> GetCategories([FromServices] CCTContext context)
        {
            var categories = context.ItemCategory.OrderBy(c => c.Id).ToList();
            return Ok(categories);
        }
    }
}