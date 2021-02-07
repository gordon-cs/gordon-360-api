using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class CliftonStrengthsViewModel
    {
        public string Theme_1 { get; set; }
        public string Theme_2 { get; set; }
        public string Theme_3 { get; set; }
        public string Theme_4 { get; set; }
        public string Theme_5 { get; set; }

        public static implicit operator CliftonStrengthsViewModel(Clifton_Strengths clif)
        {
            CliftonStrengthsViewModel vm = new CliftonStrengthsViewModel
            {
                Theme_1 = clif.THEME_1 ?? "",
                Theme_2 = clif.THEME_2 ?? "",
                Theme_3 = clif.THEME_3 ?? "",
                Theme_4 = clif.THEME_4 ?? "",
                Theme_5 = clif.THEME_5 ?? ""
            };
            return vm;
        }
    }
}
