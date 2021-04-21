using Gordon360.Static.Data;
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
        public PublicStudentProfileViewModel EditorProfile { get; set; }
        private string _editorUsername;
        public string EditorUsername
        {
            get { return EditorProfile?.AD_Username ?? _editorUsername; }
            set { _editorUsername = value; }
        }
        public string EditorEmail { get { return EditorProfile?.Email; } }
        public string Gender { get { return EditorProfile?.Gender ?? Applicants?.First()?.Profile?.Gender; } }
        public ApartmentApplicantViewModel[] Applicants { get; set; }
        public ApartmentChoiceViewModel[] ApartmentChoices { get; set; }
        public int TotalPoints { get { return Applicants?.Sum(applicant => applicant.Points) ?? 0; } }
        public double AvgPoints { get { return Applicants?.Average(applicant => applicant.Points) ?? 0; } }

        public static implicit operator ApartmentApplicationViewModel(GET_AA_APPLICATIONS_BY_ID_Result applicationDBModel)
        {
            ApartmentApplicationViewModel vm = new ApartmentApplicationViewModel
            {
                ApplicationID = applicationDBModel.AprtAppID,
                DateSubmitted = applicationDBModel.DateSubmitted,
                DateModified = applicationDBModel.DateModified,
                EditorUsername = applicationDBModel.EditorUsername,
                EditorProfile = (StudentProfileViewModel)Data.StudentData.FirstOrDefault(x => x.AD_Username.ToLower() == applicationDBModel.EditorUsername.ToLower()),
            };

            return vm;
        }
    }
}
