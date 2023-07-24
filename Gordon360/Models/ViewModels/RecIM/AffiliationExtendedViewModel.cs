using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class AffiliationExtendedViewModel
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public IEnumerable<ActivityViewModel> Activities { get; set; }


        public static implicit operator AffiliationExtendedViewModel(AffiliationPoints a)
        {
            return new AffiliationExtendedViewModel
            {
                
            };
        }
    }
}
