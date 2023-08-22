using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class MatchStatsPatchViewModel
{
    public int TeamID { get; set; }
    public int? StatusID { get; set; }
    public int? Score { get; set; }
    public int? SportsmanshipScore { get; set; }
}