using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            var result = await marketplaceService.CreateListingAsync(newListing);
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
            var result = await marketplaceService.ChangeListingStatusAsync(listingId, status);
            return Ok(result);
        }
    }
}