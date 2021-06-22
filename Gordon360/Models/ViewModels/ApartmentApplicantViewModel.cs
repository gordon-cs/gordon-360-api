using Gordon360.Static.Data;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment applicant data to/from the frontend
    public class ApartmentApplicantViewModel
    {
        public int? ApplicationID { get; set; }
        public PublicStudentProfileViewModel Profile { get; set; }
        private string _username;
        public string Username
        {
            get { return Profile?.AD_Username ?? _username; }
            set { _username = value; }
        }
        public string Class { get { return Profile?.Class ?? null; } }
        public DateTime? BirthDate { get; set; }
        public int? Age
        {
            get
            {
                if (BirthDate.HasValue)
                {
                    DateTime nextSemester = new DateTime(DateTime.Today.Year, 9, 1); //The next semester is fall of the current year, since apartment applications are only in the spring
                    return nextSemester.Year - BirthDate.Value.Year; // This age is meant to be approximate, so no need for leap-year compensation or an exact 'nextSemester' date
                }
                else { return null; }
            }
        }
        public string OffCampusProgram { get; set; }
        public bool Probation { get; set; }
        public int Points { get; set; }

        public static implicit operator ApartmentApplicantViewModel(GET_AA_APPLICANTS_BY_APPID_Result applicantDBModel)
        {
            ApartmentApplicantViewModel applicantModel = new ApartmentApplicantViewModel
            {
                ApplicationID = applicantDBModel.HousingAppID,
                Username = applicantDBModel.Username,
                // search username in cached data
                Profile = (StudentProfileViewModel)Data.StudentData.FirstOrDefault(x => x.AD_Username.ToLower() == applicantDBModel.Username.ToLower()),
                BirthDate = null, // Initialize to null. The actual value is determined and set in HousingService if and only if the user is housing admin
                OffCampusProgram = applicantDBModel.AprtProgram,
                Probation = false, // Initialize to false. The actual value is determined and set in HousingService if and only if the user is housing admin
                Points = 0, // Initialize to zero. The point actual points are calculated in HousingService
            };

            return applicantModel;
        }
    }
}
