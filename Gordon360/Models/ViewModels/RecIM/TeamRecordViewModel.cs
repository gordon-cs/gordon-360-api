using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamRecordViewModel
    {
        public int SeriesID { get; set; }
        public string Name { get; set; }
        public int WinCount { get; set; }  
        public int LossCount { get; set; }
        public int TieCount { get; set; }
        public static implicit operator TeamRecordViewModel(SeriesTeam st)
        {
            return new TeamRecordViewModel
            {
                SeriesID = st.SeriesID,
                Name = st.Team.Name,
                WinCount = st.Win,
                LossCount = st.Loss,
            };
        }
    }
}