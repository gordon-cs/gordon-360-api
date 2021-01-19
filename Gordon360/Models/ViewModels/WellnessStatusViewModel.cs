using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class DEPRECATED_WellnessStatusViewModel
    {
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public bool IsOverride { get; set; }
        public bool? IsValid { get; set; }
    }
}