using System;

namespace Gordon360.Models.ViewModels
{
    public class ApartmentApplicationsTableViewModel
    {
        public int AprtAppID { get; set; }
        public DateTime? DateSubmitted { get; set; } // Nullable
        public DateTime DateModified { get; set; }
        public string EditorID { get; set; }
    }
}