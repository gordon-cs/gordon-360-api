using System;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment application data to/from the frontend
    public class ApartmentApplicationViewModel
    {
        public int ApplicationID { get; set; }
        public DateTime? DateSubmitted { get; set; } // Nullable
        public DateTime DateModified { get; set; }
        public string EditorUsername { get; set; }
        public string EditorEmail { get; set; }
        public string Gender { get; set; }
        public ApartmentApplicantViewModel[] Applicants { get; set; }
        public ApartmentChoiceViewModel[] ApartmentChoices { get; set; }
        public int TotalPoints { get; set; }
        public double AvgPoints { get; set; }
    }
}
