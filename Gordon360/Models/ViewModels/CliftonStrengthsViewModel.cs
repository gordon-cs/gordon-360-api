using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.CCT;
using Gordon360.Static.Names;
namespace Gordon360.Models.ViewModels
{
    public class CliftonStrengthsViewModel
    {
        public List<string> Strengths { get; set; }

        public static implicit operator CliftonStrengthsViewModel(Clifton_Strengths clif)
        {
            CliftonStrengthsViewModel vm = new CliftonStrengthsViewModel
            {
                // if clif is null, just set Strengths to null to prevent exception when clif.THEME is called
                Strengths = (clif == null) ? null : new List<string>()
                {
                    clif.THEME_1,
                    clif.THEME_2,
                    clif.THEME_3,
                    clif.THEME_4,
                    clif.THEME_5,
                },
            };

            return vm;
        }
    }
}
