using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Gordon360.Services
{
    public class MarketplaceService(CCTContext context) : IMarketplaceService
    {
        /// <summary>
        /// Get all marketplace listings.
        /// </summary>
        public IEnumerable<MarketplaceListingViewModel> GetAllListings()
        {
            var listings = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .Select(item => (MarketplaceListingViewModel)item);
            return listings;
        }

        /// <summary>
        /// Get a specific marketplace listing by ID.
        /// </summary>
        public MarketplaceListingViewModel GetListingById(int listingId)
        {
            var listing = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Create a new marketplace listing.
        /// </summary>
        public async Task<MarketplaceListingViewModel> CreateListingAsync(MarketplaceListingUploadViewModel newListing)
        {
            var listing = newListing.ToPostedItem();
            context.PostedItem.Add(listing);
            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Update an existing marketplace listing.
        /// </summary>
        public async Task<MarketplaceListingViewModel> UpdateListingAsync(int listingId, MarketplaceListingUploadViewModel updatedListing)
        {
            var listing = context.PostedItem
                .Include(x => x.PostImage)
                .FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            listing.Name = updatedListing.Name;
            listing.Price = updatedListing.Price;
            listing.CategoryId = updatedListing.CategoryId;
            listing.Detail = updatedListing.Detail;
            listing.ConditionId = updatedListing.ConditionId;
            listing.StatusId = updatedListing.StatusId;

            // Update images
            listing.PostImage.Clear();
            if (updatedListing.ImagePaths != null)
            {
                foreach (var path in updatedListing.ImagePaths)
                {
                    listing.PostImage.Add(new PostImage { ImagePath = path });
                }
            }

            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Delete a marketplace listing.
        /// </summary>
        public async Task DeleteListingAsync(int listingId)
        {
            var listing = context.PostedItem.Find(listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            context.PostedItem.Remove(listing);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Change the status of a marketplace listing.
        /// </summary>
        public async Task<MarketplaceListingViewModel> ChangeListingStatusAsync(int listingId, string status)
        {
            var listing = context.PostedItem.Find(listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            listing.Status = status;
            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }
    }
}