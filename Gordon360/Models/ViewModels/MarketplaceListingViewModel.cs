using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingViewModel
    {
        public int Id { get; set; }
        public DateTime PostedAt { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Detail { get; set; }
        public int ConditionId { get; set; }
        public string ConditionName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public List<string> ImagePaths { get; set; }
        public string PosterUsername { get; set; }

        public static implicit operator MarketplaceListingViewModel(PostedItem item)
        {
            return new MarketplaceListingViewModel
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
            };
        }


        public static MarketplaceListingViewModel From(Post post)
        {
            return new MarketplaceListingViewModel
            {
                Id = post.Id,
                PostedAt = post.PostedAt,
                Name = post.Name,
                Price = post.Price,
                CategoryId = post.CategoryId,
                CategoryName = post.CategoryName,
                Detail = post.Detail,
                ConditionId = post.ConditionId,
                ConditionName = post.ConditionName,
                StatusId = post.StatusId,
                StatusName = post.StatusName,
                ImagePaths = post.ImagePaths?.Split(";")?.ToList() ?? [],
                PosterUsername = post.PostedByUsername
            };
        }
    }
}