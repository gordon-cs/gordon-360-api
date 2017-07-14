using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ActivityViewModel
    {
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }

        public static implicit operator ActivityViewModel(ACT_CLUB_DEF_DELETE a)
        {
            ActivityViewModel vm = new ActivityViewModel
            {
                ActivityCode = a.ACT_CDE.Trim(),
                ActivityDescription = a.ACT_DESC.Trim()
            };

            return vm;
        }
    }
}