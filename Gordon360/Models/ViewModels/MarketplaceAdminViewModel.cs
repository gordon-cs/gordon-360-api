using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;


namespace Gordon360.Models.ViewModels
{
    public class MarketplaceAdminViewModel
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
        public int ThreadId { get; set; } // Original post ID for reposts

        public static implicit operator MarketplaceAdminViewModel(PostedItem item)
        {
            return new MarketplaceAdminViewModel
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
                ThreadId = item.OriginalPostId ?? item.Id, // Handle null OriginalPostId
            };
        }
    }
}