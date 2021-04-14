using System;
using System.Linq;

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
        public int TotalPoints => Applicants?.Sum(applicant => applicant.Points) ?? 0;
        public double AvgPoints => Applicants?.Average(applicant => applicant.Points) ?? 0;
    }
}
