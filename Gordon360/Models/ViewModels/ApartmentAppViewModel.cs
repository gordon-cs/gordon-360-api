using System;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment application data to/from the frontend
    public class ApartmentAppViewModel
    {
        public int AprtAppID { get; set; }
        public DateTime? DateSubmitted { get; set; } // Nullable
        public DateTime DateModified { get; set; }
        public string EditorUsername { get; set; }
        public char Gender { get; set; }
        public ApartmentApplicantViewModel [] Applicants { get; set; }
        public ApartmentChoiceViewModel [] ApartmentChoices { get; set; }
    }
}