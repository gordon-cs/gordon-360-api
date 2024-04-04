using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;

namespace Gordon360.Models.ViewModels
{
    public class HousingPreferredHallViewModel
    {
        public string ApplicationID { get; set; }
        public int Rank { get; set; }
        public string HallName { get; set; }
        public static implicit operator HousingPreferredHallViewModel(PreferredHall ph)
        {
            return new HousingPreferredHallViewModel
            {
                ApplicationID = ph.ApplicationID,
                Rank = ph.Rank,
                HallName = ph.HallName
            };
        }
    }
}