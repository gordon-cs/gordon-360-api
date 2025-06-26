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
using Gordon360.Services; // For IAccountService

namespace Gordon360.Services
{
    public class MarketplaceService(
        CCTContext context,
        IWebHostEnvironment webHostEnvironment,
        ServerUtils serverUtils,
        IAccountService accountService // Inject this
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
                .Where(item => item.StatusId != 3) // Exclude statusid 3
                .OrderByDescending(item => item.Id)
                .Select(item => new MarketplaceListingViewModel
                {
                    Id = item.Id,
                    PostedAt = item.PostedAt,
                    Name = item.Name,
                    Price = item.Price,
                    CategoryId = item.CategoryId,
                    CategoryName = item.Category.CategoryName,
                    Detail = item.Detail,
                    ConditionId = item.ConditionId,
                    ConditionName = item.Condition.ConditionName,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    ImagePaths = item.PostImage.Select(img => img.ImagePath).ToList(),
                    PosterUsername = context.ACCOUNT
                        .Where(a => a.gordon_id == item.PostedById.ToString())
                        .Select(a => a.AD_Username)
                        .FirstOrDefault()
                })
                .ToList();
            return listings;
        }

        public IEnumerable<MarketplaceListingViewModel> GetUserListings(string username)
        {
            var account = accountService.GetAccountByUsername(username);
            if (account == null) return new List<MarketplaceListingViewModel>();

            int userId = int.Parse(account.GordonID);

            var listings = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .Where(item => item.PostedById == userId)
                .Where(item => item.StatusId != 3) // Exclude statusid 3
                .OrderByDescending(item => item.Id)
                .Select(item => new MarketplaceListingViewModel
                {
                    Id = item.Id,
                    PostedAt = item.PostedAt,
                    Name = item.Name,
                    Price = item.Price,
                    CategoryId = item.CategoryId,
                    CategoryName = item.Category.CategoryName,
                    Detail = item.Detail,
                    ConditionId = item.ConditionId,
                    ConditionName = item.Condition.ConditionName,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    ImagePaths = item.PostImage.Select(img => img.ImagePath).ToList(),
                    PosterUsername = context.ACCOUNT
                        .Where(a => a.gordon_id == item.PostedById.ToString())
                        .Select(a => a.AD_Username)
                        .FirstOrDefault()
                })
                .ToList();

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

            var account = context.ACCOUNT.FirstOrDefault(a => a.gordon_id == listing.PostedById.ToString());

            return new MarketplaceListingViewModel
            {
                Id = listing.Id,
                PostedAt = listing.PostedAt,
                Name = listing.Name,
                Price = listing.Price,
                CategoryId = listing.CategoryId,
                CategoryName = listing.Category?.CategoryName,
                Detail = listing.Detail,
                ConditionId = listing.ConditionId,
                ConditionName = listing.Condition?.ConditionName,
                StatusId = listing.StatusId,
                StatusName = listing.Status?.StatusName,
                ImagePaths = listing.PostImage?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                PosterUsername = account?.AD_Username
            };
        }

        /// <summary>
        /// Create a new marketplace listing.
        /// </summary>
        public async Task<MarketplaceListingViewModel> CreateListingAsync(MarketplaceListingUploadViewModel newListing, string username)
        {
            var account = accountService.GetAccountByUsername(username);
            if (account == null)
                throw new Exception("Account not found");

            var listing = new PostedItem
            {
                PostedById = int.Parse(account.GordonID), // Set automatically
                Name = newListing.Name,
                Price = newListing.Price,
                CategoryId = newListing.CategoryId,
                Detail = newListing.Detail,
                ConditionId = newListing.ConditionId,
                StatusId = 1,
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
                //.Include(x => x.PostImage)
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

            // Remove old images from disk if needed
            //foreach (var img in listing.PostImage)
            //{
            //    if (!string.IsNullOrEmpty(img.ImagePath))
            //    {
            //        ImageUtils.DeleteImage(img.ImagePath);
            //    }
            //}
            //listing.PostImage.Clear();

            //if (updatedListing.ImagesBase64 != null)
            //{
            //    foreach (var base64 in updatedListing.ImagesBase64)
            //    {
            //        if (!string.IsNullOrWhiteSpace(base64) && base64.StartsWith("data:image"))
            //        {
            //            var (extension, format, data) = ImageUtils.GetImageFormat(base64);
            //            var filename = $"{Guid.NewGuid():N}.{extension}";
            //            var imagePath = GetImagePath(filename); // Save to disk
            //            var url = GetImageURL(filename);        // Store this in DB
            //            ImageUtils.UploadImage(imagePath, data, format);
            //            listing.PostImage.Add(new PostImage { ImagePath = url });
            //        }
            //    }
            //}

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
            listing.PostedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Get filtered marketplace listings based on criteria.
        /// </summary>
        public IEnumerable<MarketplaceListingViewModel> GetFilteredListings(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string search = null, string sortBy = null, bool desc = false,
            int page = 1, int pageSize = 20)
        {
            var query = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .Where(x => x.StatusId != 3)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            if (statusId.HasValue)
                query = query.Where(x => x.StatusId == statusId.Value);

            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.Detail.Contains(search));

            // Sorting
            switch (sortBy?.ToLower())
            {
                case "price":
                    query = desc ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                    break;
                case "date":
                case "postedat":
                    query = desc ? query.OrderByDescending(x => x.PostedAt) : query.OrderBy(x => x.PostedAt);
                    break;
                case "name":
                    query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                    break;
                default:
                    query = query.OrderByDescending(x => x.PostedAt); // Default: newest first
                    break;
            }

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return query.Select(item => new MarketplaceListingViewModel
            {
                Id = item.Id,
                PostedAt = item.PostedAt,
                Name = item.Name,
                Price = item.Price,
                CategoryId = item.CategoryId,
                CategoryName = item.Category.CategoryName,
                Detail = item.Detail,
                ConditionId = item.ConditionId,
                ConditionName = item.Condition.ConditionName,
                StatusId = item.StatusId,
                StatusName = item.Status.StatusName,
                ImagePaths = item.PostImage.Select(img => img.ImagePath).ToList(),
                PosterUsername = context.ACCOUNT
                    .Where(a => a.gordon_id == item.PostedById.ToString())
                    .Select(a => a.AD_Username)
                    .FirstOrDefault()
            }).ToList();
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