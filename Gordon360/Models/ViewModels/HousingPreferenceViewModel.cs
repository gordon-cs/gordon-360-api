using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.ViewModels
{
    public class HousingPreferenceViewModel
    {
        public string ApplicationID { get; set; }
        public string Preference1 { get; set; }
        public static implicit operator HousingPreferenceViewModel(Preference p)
        {
            return new HousingPreferenceViewModel
            {
                ApplicationID = p.ApplicationID,
                Preference1 = p.Preference1
            };
        }
    }
}