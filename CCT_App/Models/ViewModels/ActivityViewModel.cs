using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCT_App.Models.ViewModels
{
    public class ActivityViewModel
    {
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }

        public static implicit operator ActivityViewModel(ACT_CLUB_DEF a)
        {
            ActivityViewModel vm = new ActivityViewModel
            {
                ActivityCode = a.ACT_CDE,
                ActivityDescription = a.ACT_DESC
            };

            return vm;
        }
    }
}