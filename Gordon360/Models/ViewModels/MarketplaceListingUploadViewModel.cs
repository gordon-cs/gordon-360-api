using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingUploadViewModel
    {
        public string UserId { get; set; }
        public string ItemName { get; set; }
        public int ItemPrice { get; set; }
        public string ItemCategory { get; set; }
        public string ItemDetail { get; set; }
        public string Condition { get; set; }
        public string ItemStatus { get; set; }
        public string ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; } // Nullable to allow null values
        public string? ImagePath3 { get; set; } // Nullable to allow null values

        /// <summary>
        /// Converts the upload view model to a PostedItem entity.
        /// Handles null values for ImagePath2 and ImagePath3.
        /// </summary>
        public PostedItem ToPostedItem()
        {
            return new PostedItem
            {
                UserId = this.UserId,
                ItemName = this.ItemName,
                ItemPrice = this.ItemPrice,
                ItemCategory = this.ItemCategory,
                ItemDetail = this.ItemDetail,
                Condition = this.Condition,
                ItemStatus = this.ItemStatus,
                ImagePath1 = this.ImagePath1,
                ImagePath2 = this.ImagePath2 ?? string.Empty, // Default to empty string if null
                ImagePath3 = this.ImagePath3 ?? string.Empty  // Default to empty string if null
            };
        }
    }
}