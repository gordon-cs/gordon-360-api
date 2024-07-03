using Gordon360.Models.CCT;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class MatchBracketExportViewModel
{
    public int id { get; set; }
    public string? name { get; set; }
    public int? nextMatchID { get; set; }
    public string tournamentRoundText { get; set; }
    public string? state { get; set; }
    public DateTime startTime { get; set;}
    public IEnumerable<TeamBracketExportViewModel> participants { get; set; }
    public int seedIndex { get; set; }
    public bool isLosers { get; set; }
}