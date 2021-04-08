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
        public StudentProfileViewModel EditorProfile { get; set; }
        private string _editorUsername;
        public string EditorUsername
        {
            get => EditorProfile?.AD_Username ?? _editorUsername;
            set => _editorUsername = value;
        }
        public string EditorEmail => EditorProfile?.Email;
        public string Gender => EditorProfile?.Gender ?? Applicants?.First()?.Profile?.Gender;
        public ApartmentApplicantViewModel[] Applicants { get; set; }
        public ApartmentChoiceViewModel[] ApartmentChoices { get; set; }
        public int TotalPoints => Applicants?.Sum(applicant => applicant.Points) ?? 0;
        public double AvgPoints => Applicants?.Average(applicant => applicant.Points) ?? 0;

        public static implicit operator ApartmentApplicationViewModel(GET_AA_APPLICATIONS_BY_ID_Result applicationDBModel)
        {
            ApartmentApplicationViewModel vm = new ApartmentApplicationViewModel
            {
                ApplicationID = applicationDBModel.AprtAppID,
                DateSubmitted = applicationDBModel.DateSubmitted,
                DateModified = applicationDBModel.DateModified,
                //EditorUsername = applicationDBModel.EditorUsername, // Code for after we remade the AA_Applications table
            };

            return vm;
        }
    }
}
