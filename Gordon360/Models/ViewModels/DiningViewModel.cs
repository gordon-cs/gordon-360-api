using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class DiningViewModel
    {
        public string ChoiceDescription { get; set; }
        public string PlanDescriptions { get; set; }
        public string PlanId { get; set; }

        public int InitialBalance { get; set; }

        public string CurrentBalance { get; set; }
    }
}