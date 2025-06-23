using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingUploadViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string Detail { get; set; }
        public int ConditionId { get; set; }
        public int StatusId { get; set; }
        // Accept base64 images from frontend
        public List<string> ImagesBase64 { get; set; }
    }
}