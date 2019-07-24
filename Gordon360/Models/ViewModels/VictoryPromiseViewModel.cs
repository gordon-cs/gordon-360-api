using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class VictoryPromiseViewModel
    {
        // a separate int for each score
        public Nullable<int> TOTAL_VP_IM_SCORE { get; set; } // Intellectual Maturity 
        public Nullable<int> TOTAL_VP_CC_SCORE { get; set; } // Christian Character
        public Nullable<int> TOTAL_VP_LS_SCORE { get; set; } // Lives of Service
        public Nullable<int> TOTAL_VP_LW_SCORE { get; set; } // Leadership Worldwide
    }
}