using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class HousingAppToSubmitViewModel
    {
        public int AprtAppID { get; set; }
        public Boolean Submitted { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DateModified { get; set; }
        public int ModifiedBy { get; set; }
        public int ModifierID { get; set; }
    }
}