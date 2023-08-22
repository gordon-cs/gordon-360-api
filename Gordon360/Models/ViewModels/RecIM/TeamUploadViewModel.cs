using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamUploadViewModel
{
    public string Name { get; set; }
    public int ActivityID { get; set; }
    public string? Logo { get; set; }
    public Team ToTeam()
    {
        return new Team
        {
            Name = this.Name,
            StatusID = 1, //unconfirmed 
            ActivityID = this.ActivityID,
            Logo = this.Logo,
        };
    }
}
