using System;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment applicant data to/from the frontend
    public class ApartmentApplicantViewModel
    {
        public int ApplicationID { get; set; }
        public StudentProfileViewModel Profile { get; set; } // We need a way to make this StudentProfileViewModel if the is authorized to view this user profile, otherwise it must be PublicStudentProfileViewModel
        private string _username;
        public string Username
        {
            get => Profile?.AD_Username ?? _username;
            set => _username = value;
        }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; } // To be calculated from the birthday
        public string Class => Profile?.Class ?? null;
        public string OffCampusProgram { get; set; }
        public bool Probation { get; set; }
        public int Points { get; set; }

        public static implicit operator ApartmentApplicantViewModel(GET_AA_APPLICANTS_BY_APPID_Result applicantDBModel)
        {
            ApartmentApplicantViewModel vm = new ApartmentApplicantViewModel
            {
                ApplicationID = applicantDBModel.AprtAppID,
                StudentID = applicantDBModel.ID_NUM,
                //Username = applicantDBModel.Username, // Code for after we remade the AA_Applicants table
            };

            return vm;
        }
    }
}