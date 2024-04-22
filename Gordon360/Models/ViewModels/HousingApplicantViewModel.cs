using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;

namespace Gordon360.Models.ViewModels
{
    public class HousingApplicantViewModel
    {
        public string ApplicationID { get; set; }
        public string Email { get; set; }
        public static implicit operator HousingApplicantViewModel(Applicant a)
        {
            return new HousingApplicantViewModel
            {
                ApplicationID = a.ApplicationID,
                Email = a.Email
            };
        }
    }
}