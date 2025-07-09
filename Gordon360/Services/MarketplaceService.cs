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
                .OrderByDescending(item => item.PostedAt)
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
                .OrderByDescending(item => item.PostedAt)
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
                DeletedAt = DateTime.Now.AddDays(90),
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
        public async Task<MarketplaceListingViewModel> UpdateListingAsync(int listingId, MarketplaceListingUpdateViewModel updatedListing)
        {
            var oldListing = context.PostedItem
                .Include(x => x.PostImage)
                .FirstOrDefault(x => x.Id == listingId);
            if (oldListing == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "Listing not found." };
            }

            // Create new listing with updated info, but keep original PostedAt
            var newListing = new PostedItem
            {
                PostedById = oldListing.PostedById,
                PostedAt = oldListing.PostedAt,
                DeletedAt = DateTime.Now.AddDays(90),
                Name = updatedListing.Name,
                Price = updatedListing.Price,
                CategoryId = updatedListing.CategoryId,
                Detail = updatedListing.Detail,
                ConditionId = updatedListing.ConditionId,
                StatusId = oldListing.StatusId,
                PostImage = new List<PostImage>(),
            };

            newListing.OriginalPostId = oldListing.OriginalPostId ?? oldListing.Id;

            context.PostedItem.Add(newListing);
            await context.SaveChangesAsync(); // Save to get newListing.Id

            // Move images from old post to new post
            var images = context.PostImage.Where(img => img.PostedItemId == oldListing.Id).ToList();
            foreach (var img in images)
            {
                img.PostedItemId = newListing.Id;
            }

            oldListing.StatusId = 3;
            oldListing.DeletedAt = DateTime.Now; // Mark old listing as deleted

            await context.SaveChangesAsync();

            return (MarketplaceListingViewModel)newListing;
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

            // Set DeletedAt based on new status
            if (statusEntity.StatusName.Equals("Sold", StringComparison.OrdinalIgnoreCase))
            {
                listing.DeletedAt = DateTime.Now.AddDays(30); // Sold: 30 days from now
            }
            else if (statusEntity.StatusName.Equals("For Sale", StringComparison.OrdinalIgnoreCase))
            {
                listing.DeletedAt = listing.PostedAt.AddDays(90);
            }
            else if (statusEntity.StatusName.Equals("Deleted", StringComparison.OrdinalIgnoreCase))
            {
                listing.DeletedAt = DateTime.Now; // Deleted: now
            }

            await context.SaveChangesAsync();
            return (MarketplaceListingViewModel)listing;
        }

        /// <summary>
        /// Get filtered marketplace listings based on criteria.
        /// </summary>
        public IEnumerable<MarketplaceListingViewModel> GetFilteredListings(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search, string? sortBy, bool desc = false,
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
                    query = desc ? query.OrderByDescending(x => x.PostedAt) : query.OrderBy(x => x.PostedAt);
                    break;
                case "title":
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


        public int GetFilteredListingsCount(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search)
        {
            var query = context.PostedItem
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

            return query.Count();
        }

        /// <summary>
        /// Get all marketplace threads for admin view (one row per thread, including deleted/expired).
        /// </summary>
        public IEnumerable<MarketplaceAdminViewModel> GetAdminThreads(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search, string? sortBy, bool desc = false,
            int page = 1, int pageSize = 20)
        {
            var query = context.PostedItem
                .Include(x => x.Category)
                .Include(x => x.Condition)
                .Include(x => x.Status)
                .Include(x => x.PostImage)
                .AsQueryable();

            // Filtering
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

            // Group by thread (OriginalPostId or Id if null), select latest version per thread
            var threads = query
                .AsEnumerable() // Grouping with navigation properties must be done in memory
                .GroupBy(x => x.OriginalPostId ?? x.Id)
                .Select(g => g.OrderByDescending(x => x.Id).First());

            // Sorting
            switch (sortBy?.ToLower())
            {
                case "price":
                    threads = desc ? threads.OrderByDescending(x => x.Price) : threads.OrderBy(x => x.Price);
                    break;
                case "date":
                    threads = desc ? threads.OrderByDescending(x => x.PostedAt) : threads.OrderBy(x => x.PostedAt);
                    break;
                case "title":
                    threads = desc ? threads.OrderByDescending(x => x.Name) : threads.OrderBy(x => x.Name);
                    break;
                default:
                    threads = threads.OrderByDescending(x => x.PostedAt); // Default: newest first
                    break;
            }

            // Pagination
            threads = threads.Skip((page - 1) * pageSize).Take(pageSize);

            return threads.Select(item => new MarketplaceAdminViewModel
            {
                Id = item.Id,
                PostedAt = item.PostedAt,
                Name = item.Name,
                Price = item.Price,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.CategoryName,
                Detail = item.Detail,
                ConditionId = item.ConditionId,
                ConditionName = item.Condition?.ConditionName,
                StatusId = item.StatusId,
                StatusName = item.Status?.StatusName,
                ImagePaths = item.PostImage?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                PosterUsername = context.ACCOUNT
                    .Where(a => a.gordon_id == item.PostedById.ToString())
                    .Select(a => a.AD_Username)
                    .FirstOrDefault(),
                ThreadId = item.OriginalPostId ?? item.Id
                
            }).ToList();
        }

        /// <summary>
        /// Get the edit history for a thread (all versions, oldest to newest).
        /// </summary>
        public IEnumerable<MarketplaceListingViewModel> GetThreadEditHistory(int threadId)
        {
            var history = context.PostedItem
              .Include(x => x.Category)
              .Include(x => x.Condition)
              .Include(x => x.Status)
              .Include(x => x.PostImage)
              .Where(x => x.OriginalPostId == threadId || x.Id == threadId)
              .OrderBy(x => x.Id)
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

            return history;
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
            return $"browseable/uploads/marketplace/images/{filename}";
        }

        public async Task<ItemCategory> AddCategoryAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty.");

            categoryName = categoryName.Trim();

            if (categoryName.Length > 50)
                throw new ArgumentException("Category name must be 50 characters or fewer.");
            if (!categoryName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                throw new ArgumentException("Category name can only contain letters, numbers, and spaces.");

            bool exists = context.ItemCategory
                .Any(c => c.CategoryName.ToLower() == categoryName.ToLower());
            if (exists)
                throw new ArgumentException("A category with this name already exists.");

            var category = new ItemCategory { CategoryName = categoryName, Visible = "Y" };
            context.ItemCategory.Add(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<ItemCondition> AddConditionAsync(string conditionName)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new ArgumentException("Condition name cannot be empty.");

            conditionName = conditionName.Trim();

            if (conditionName.Length > 50)
                throw new ArgumentException("Category name must be 50 characters or fewer.");
            if (!conditionName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                throw new ArgumentException("Category name can only contain letters, numbers, and spaces.");

            bool exists = context.ItemCondition
                .Any(c => conditionName.ToLower() == conditionName.ToLower());
            if (exists)
                throw new ArgumentException("A condition with this name already exists.");


            var condition = new ItemCondition { ConditionName = conditionName, Visible = "Y" };
            context.ItemCondition.Add(condition);
            await context.SaveChangesAsync();
            return condition;
        }

        public async Task<ItemCategory> UpdateCategoryVisibilityAsync(string categoryName, bool visible)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty.");
            var category = context.ItemCategory.FirstOrDefault(c => c.CategoryName == categoryName);
            if (category == null)
                throw new ResourceNotFoundException { ExceptionMessage = "Category not found." };
            category.Visible = visible ? "Y" : "N";
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<ItemCondition> UpdateConditionVisibilityAsync(string conditionName, bool visible)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new ArgumentException("Condition name cannot be empty.");
            var condition = context.ItemCondition.FirstOrDefault(c => c.ConditionName == conditionName);
            if (condition == null)
                throw new ResourceNotFoundException { ExceptionMessage = "Condition not found." };
            condition.Visible = visible ? "Y" : "N";
            await context.SaveChangesAsync();
            return condition;
        }

        public int GetAdminThreadsCount(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search)
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

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.Detail.Contains(search));

            // Group by thread and count unique threads
            var count = query
                .AsEnumerable()
                .GroupBy(x => x.OriginalPostId ?? x.Id)
                .Count();

            return count;
        }
    }
}