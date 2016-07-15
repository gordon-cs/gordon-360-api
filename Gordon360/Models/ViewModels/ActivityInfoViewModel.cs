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
        public string MeetingDay { get; set; }
        public string MeetingTime { get; set; }
        public string ActivityImage { get; set; }

        public static implicit operator ActivityInfoViewModel(Activity_Info info)
        {
            ActivityInfoViewModel vm = new ActivityInfoViewModel
            {
                ActivityCode = info.ACT_CDE.Trim(),
                ActivityDescription = info.ACT_DESCR.Trim(),
                MeetingDay = info.MTG_DAY.Trim(),
                MeetingTime = info.MTG_TIME.Trim(),
                ActivityImage = info.ACT_IMAGE.Trim()
            };

            return vm;
        }
    }

   
}