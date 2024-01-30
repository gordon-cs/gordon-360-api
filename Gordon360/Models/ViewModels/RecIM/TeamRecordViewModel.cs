using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamRecordViewModel
{
    public int SeriesID { get; set; }
    public int TeamID { get; set; }
    public string? Logo { get; set; }
    public string Name { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public int TieCount { get; set; }
    public double? SportsmanshipRating { get; set; } // has to be nullable due to calculation query possibly propagating null
    public static implicit operator TeamRecordViewModel(SeriesTeam st)
    {
        return new TeamRecordViewModel
        {
            SeriesID = st.SeriesID,
            TeamID = st.TeamID,
            Name = st.Team.Name,
            WinCount = st.WinCount,
            LossCount = st.LossCount,
        };
    }
}