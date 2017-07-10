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
        public string ActivityImagePath { get; set; }
        public string ActivityBlurb { get; set; }
        public string ActivityURL { get; set; }
        public string ActivityType { get; set; }
        public string ActivityTypeDescription { get; set; }
        public string ActivityJoinInfo { get; set; }

        public static implicit operator ActivityInfoViewModel(ACT_INFO info)
        {
            ActivityInfoViewModel vm = new ActivityInfoViewModel
            {
                ActivityCode = info.ACT_CDE.Trim(),
                ActivityDescription = info.ACT_DESC.Trim() ?? "",
                ActivityBlurb = info.ACT_BLURB ?? "",
                ActivityURL = info.ACT_URL ?? "",
                ActivityImagePath = info.ACT_IMG_PATH ?? "",
                ActivityType = info.ACT_TYPE ?? "",
                ActivityTypeDescription = info.ACT_TYPE_DESC.Trim() ?? "",
                ActivityJoinInfo = info.ACT_JOIN_INFO.Trim() ?? ""
            };

            return vm;
        }
    }

   
}