using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ActivityInfoViewModel
    {
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityImage { get; set; }
        public string ActivityBlurb { get; set; }
        public string ActivityURL { get; set; }

        public static implicit operator ActivityInfoViewModel(ACT_INFO info)
        {
            ActivityInfoViewModel vm = new ActivityInfoViewModel
            {
                ActivityCode = info.ACT_CDE.Trim(),
                ActivityDescription = info.ACT_DESC.Trim() ?? "",
                ActivityImage = info.ACT_IMAGE.Trim() ?? "",
                ActivityBlurb = info.ACT_BLURB ?? "",
                ActivityURL = info.ACT_URL ?? ""
            };

            return vm;
        }
    }

   
}