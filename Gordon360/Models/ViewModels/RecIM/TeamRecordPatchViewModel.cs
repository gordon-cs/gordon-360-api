using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamRecordPatchViewModel
{
    public int TeamID { get; set; }
    public int? WinCount { get; set; }  
    public int? LossCount { get; set; }
    public int? TieCount { get; set; }

}