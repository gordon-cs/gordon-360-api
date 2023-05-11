using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment applicant data to/from the frontend
    public class ApartmentApplicantViewModel
    {
        public int? ApplicationID { get; set; }
        public PublicStudentProfileViewModel? Profile { get; set; }
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
                    var age = nextSemester.Year - BirthDate.Value.Year; // This age is meant to be approximate, so no need for leap-year compensation or an exact 'nextSemester' date
                    return (BirthDate.Value.Date > nextSemester.AddYears(-age)) ? age - 1 : age; // If birth date is after the start of next semester, they are one year younger.
                }
                else { return null; }
            }
        }
        public string OffCampusProgram { get; set; }
        public bool Probation { get; set; }
        public int Points { get; set; }

        public static implicit operator ApartmentApplicantViewModel(Housing_Applicants applicantDBModel) => new ApartmentApplicantViewModel
        {
            ApplicationID = applicantDBModel.HousingAppID,
            Username = applicantDBModel.Username, // search username in cached data
            BirthDate = null, // Initialize to null. The actual value is determined and set in HousingService if and only if the user is housing admin
            OffCampusProgram = applicantDBModel.AprtProgram,
            Probation = false, // Initialize to false. The actual value is determined and set in HousingService if and only if the user is housing admin
            Points = 0, // Initialize to zero. The point actual points are calculated in HousingService
        };
    }
}
