using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class DiningViewModel
    {
        public string MealChoiceID { get; set; }

        public string[] PlanIDs { get; set; }

        public string[] ChoiceDescription { get; set; }
        public string[] PlanDescriptions { get; set; }

        public string[] InitialBalance { get; set; }

        public string[] CurrentBalance { get; set; }
    }
}