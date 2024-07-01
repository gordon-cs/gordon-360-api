using Gordon360.Models.CCT;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class MatchBracketExtendedViewModel
{
    public int MatchID { get; set; }
    public int? NextMatchID { get; set; }
    public int RoundNumber { get; set; }
    public int RoundOf { get; set; }
    public string? State { get; set; }
    public IEnumerable<TeamExtendedViewModel> Team { get; set; }
    public int SeedIndex { get; set; }
    public bool IsLosers { get; set; }
}