using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int StatusID { get; set; }
    public int ActivityID { get; set; }
    public string? Logo { get; set; }
    public string? Affiliation { get; set; }
    public static implicit operator TeamViewModel(Team t)
    {
        return new TeamViewModel
        {
            ID = t.ID,
            Name = t.Name,
            StatusID = t.StatusID,
            ActivityID = t.ActivityID,
            Logo = t.Logo,
            Affiliation = t.Affiliation
        };
    }
}