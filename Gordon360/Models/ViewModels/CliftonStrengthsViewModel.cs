using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class CliftonStrengthsViewModel
    {
        public int test { get; set; }
        public List<string> Strengths { get; set; }

        public static implicit operator CliftonStrengthsViewModel(Clifton_Strengths clif)
        {
            CliftonStrengthsViewModel vm = new CliftonStrengthsViewModel
            {
                Strengths = new List<string>()
                {
                    clif?.THEME_1,
                    clif?.THEME_2,
                    clif?.THEME_3,
                    clif?.THEME_4,
                    clif?.THEME_5
                }
            };
            return vm;
        }
    }
}
