using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.ViewModels
{
    public class HousingYearViewModel
    {
        public string ApplicationID { get; set; }
        public string Year { get; set; }
        public HousingYearViewModel (Student s, Applicant a)
        {
            ApplicationID = a.ApplicationID;
            Year = s.Class;
        }
        public HousingYearViewModel (string applicationID, string year)
        {
            ApplicationID = applicationID;
            Year = year;
        }
    }
}