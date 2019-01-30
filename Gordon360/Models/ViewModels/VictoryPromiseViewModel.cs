using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class VictoryPromiseViewModel
    {
        // a separate int for each score
        public int im { get; set; } // Intellectual Maturity 
        public int cc { get; set; } // Christian Character
        public int lv { get; set; } // Lives of Service
        public int lw { get; set; } // Leadership Worldwide
    }
}