using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Gordon360.Utilities; // For ImageUtils

namespace Gordon360.Services
{
    public class MarketplaceService(
        CCTContext context,
        IWebHostEnvironment webHostEnvironment,
        ServerUtils serverUtils
    ) : IMarketplaceService
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
            var listing = new PostedItem
            {
                PostedById = newListing.PostedById,
                Name = newListing.Name,
                Price = newListing.Price,
                CategoryId = newListing.CategoryId,
                Detail = newListing.Detail,
                ConditionId = newListing.ConditionId,
                StatusId = newListing.StatusId,
                PostImage = new List<PostImage>()
            };

            if (newListing.ImagesBase64 != null)
            {
                foreach (var base64 in newListing.ImagesBase64)
                {
                    if (!string.IsNullOrWhiteSpace(base64) && base64.StartsWith("data:image"))
                    {
                        var (extension, format, data) = ImageUtils.GetImageFormat(base64);
                        var filename = $"{Guid.NewGuid():N}.{extension}";
                        var imagePath = GetImagePath(filename); // Save to disk
                        var url = GetImageURL(filename);        // Store this in DB
                        ImageUtils.UploadImage(imagePath, data, format);
                        listing.PostImage.Add(new PostImage { ImagePath = url });
                    }
                }
            }

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

            // Remove old images from disk if needed
            foreach (var img in listing.PostImage)
            {
                if (!string.IsNullOrEmpty(img.ImagePath))
                {
                    ImageUtils.DeleteImage(img.ImagePath);
                }
            }
            listing.PostImage.Clear();

            if (updatedListing.ImagesBase64 != null)
            {
                foreach (var base64 in updatedListing.ImagesBase64)
                {
                    if (!string.IsNullOrWhiteSpace(base64) && base64.StartsWith("data:image"))
                    {
                        var (extension, format, data) = ImageUtils.GetImageFormat(base64);
                        var filename = $"{Guid.NewGuid():N}.{extension}";
                        var imagePath = GetImagePath(filename); // Save to disk
                        var url = GetImageURL(filename);        // Store this in DB
                        ImageUtils.UploadImage(imagePath, data, format);
                        listing.PostImage.Add(new PostImage { ImagePath = url });
                    }
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
            var listing = context.PostedItem
                .Include(x => x.PostImage)
                .FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            // Delete images from disk
            foreach (var img in listing.PostImage)
            {
                if (!string.IsNullOrEmpty(img.ImagePath))
                {
                    ImageUtils.DeleteImage(img.ImagePath);
                }
            }

            context.PostedItem.Remove(listing);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Change the status of a marketplace listing.
        /// </summary>
        public async Task<MarketplaceListingViewModel> ChangeListingStatusAsync(int listingId, string status)
        {
            var listing = context.PostedItem
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            var statusEntity = context.ItemStatus.FirstOrDefault(s => s.StatusName == status);
            if (statusEntity == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Status not found." };
            }

            listing.StatusId = statusEntity.Id;
            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Get filtered marketplace listings based on criteria.
        /// </summary>
        public IEnumerable<MarketplaceListingViewModel> GetFilteredListings(int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice)
        {
            var query = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            if (statusId.HasValue)
                query = query.Where(x => x.StatusId == statusId.Value);

            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice.Value);

            return query.Select(item => (MarketplaceListingViewModel)item).ToList();
        }

        private string GetImagePath(string filename)
        {
            var directoryPath = Path.Combine(webHostEnvironment.ContentRootPath, "browseable", "uploads", "marketplace", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return Path.Combine(directoryPath, filename);
        }

        private string GetImageURL(string filename)
        {
            // If you want a full URL, use serverUtils.GetAddress() here
            // var serverAddress = serverUtils.GetAddress();
            // return $"{serverAddress}/browseable/uploads/marketplace/images/{filename}";
            return $"browseable/uploads/marketplace/images/{filename}";
        }
    }
}