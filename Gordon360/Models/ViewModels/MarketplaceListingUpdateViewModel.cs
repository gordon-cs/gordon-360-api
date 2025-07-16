using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class MarketplaceListingUpdateViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string Detail { get; set; }
        public int ConditionId { get; set; }
    }
}