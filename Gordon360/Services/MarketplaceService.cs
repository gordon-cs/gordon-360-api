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
            var listings = context.PostedItem.Select(item => (MarketplaceListingViewModel)item);
            return listings;
        }

        /// <summary>
        /// Get a specific marketplace listing by ID.
        /// </summary>
        public MarketplaceListingViewModel GetListingById(int listingId)
        {
            var listing = context.PostedItem.Find(listingId);
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
            var listing = context.PostedItem.Find(listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            listing.ItemName = updatedListing.ItemName;
            listing.ItemPrice = updatedListing.ItemPrice;
            listing.ItemCategory = updatedListing.ItemCategory;
            listing.ItemDetail = updatedListing.ItemDetail;
            listing.Condition = updatedListing.Condition;
            listing.ItemStatus = updatedListing.ItemStatus;
            listing.ImagePath1 = updatedListing.ImagePath1;
            listing.ImagePath2 = updatedListing.ImagePath2 ?? string.Empty; // Handle null values
            listing.ImagePath3 = updatedListing.ImagePath3 ?? string.Empty; // Handle null values

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

            listing.ItemStatus = status;
            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }
    }
}