using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment application data to/from the frontend
    public class ApartmentApplicationViewModel
    {
        public int? ApplicationID { get; set; } // '?' means Nullable
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateModified { get; set; }
        public PublicStudentProfileViewModel EditorProfile { get; set; }
        private string _editorUsername;
        public string EditorUsername
        {
            get { return EditorProfile?.AD_Username ?? _editorUsername; }
            set { _editorUsername = value; }
        }
        public string EditorEmail { get { return EditorProfile?.Email; } }
        public string Gender { get { return EditorProfile?.Gender ?? Applicants?.First()?.Profile?.Gender; } }
        public List<ApartmentApplicantViewModel> Applicants { get; set; }
        public List<ApartmentChoiceViewModel> ApartmentChoices { get; set; }
        public int TotalPoints { get { return Applicants?.Sum(applicant => applicant.Points) ?? 0; } }
        public double AvgPoints { get { return Math.Round(Applicants?.Average(applicant => applicant.Points) ?? 0, 2); } }

        public static implicit operator ApartmentApplicationViewModel(Housing_Applications application) => new ApartmentApplicationViewModel
        {
            ApplicationID = application.HousingAppID,
            DateSubmitted = application.DateSubmitted,
            DateModified = application.DateModified,
            EditorUsername = application.EditorUsername,
        };

    }
}
