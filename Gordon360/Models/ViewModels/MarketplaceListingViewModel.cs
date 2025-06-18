using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingViewModel
    {
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public string ItemName { get; set; }
        public int ItemPrice { get; set; }
        public string ItemCategory { get; set; }
        public string ItemDetail { get; set; }
        public string Condition { get; set; }
        public string ItemStatus { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }

        public static implicit operator MarketplaceListingViewModel(PostedItem item)
        {
            return new MarketplaceListingViewModel
            {
                ItemId = item.ItemId,
                UserId = item.UserId,
                ItemName = item.ItemName,
                ItemPrice = item.ItemPrice,
                ItemCategory = item.ItemCategory,
                ItemDetail = item.ItemDetail,
                Condition = item.Condition,
                ItemStatus = item.ItemStatus,
                ImagePath1 = item.ImagePath1,
                ImagePath2 = item.ImagePath2,
                ImagePath3 = item.ImagePath3
            };
        }
    }
}