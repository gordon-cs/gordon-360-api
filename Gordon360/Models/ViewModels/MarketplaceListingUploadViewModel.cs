using System.Collections.Generic;
using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingUploadViewModel
    {
        public int PostedById { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string Detail { get; set; }
        public int ConditionId { get; set; }
        public int StatusId { get; set; }
        public List<string> ImagePaths { get; set; }

        public PostedItem ToPostedItem()
        {
            var postedItem = new PostedItem
            {
                PostedById = this.PostedById,
                Name = this.Name,
                Price = this.Price,
                CategoryId = this.CategoryId,
                Detail = this.Detail,
                ConditionId = this.ConditionId,
                StatusId = this.StatusId,
                PostImage = this.ImagePaths?.Select(path => new PostImage { ImagePath = path }).ToList() ?? new List<PostImage>()
            };
            return postedItem;
        }
    }
}