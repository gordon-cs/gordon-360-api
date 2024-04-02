using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class AffiliationPointsViewModel
{
    public int TeamID { get; set; }
    public int SeriesID { get; set; }
    public int Points { get; set; }

    public static implicit operator AffiliationPointsViewModel(AffiliationPoints ap)
    {
        return new AffiliationPointsViewModel
        {
            TeamID = ap.TeamID,
            SeriesID = ap.SeriesID,
            Points = ap.Points
        };
    }
}
