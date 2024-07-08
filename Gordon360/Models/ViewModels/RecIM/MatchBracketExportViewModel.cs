using Gordon360.Models.CCT;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

// MatchBracketExportViewModel is essentially a renamed version of MatchBracketExtendedViewModel with name 
// changes to mirror exactly that of the UI. As well as a type change from int -> string for tournamentRoundText
// This is purely for ease of access from the UI side and lighter calculations on the UI side.
public class MatchBracketExportViewModel
{
    public int id { get; set; }
    public string? name { get; set; }
    public int? nextMatchId { get; set; }
    public string tournamentRoundText { get; set; }
    public string? state { get; set; }
    public DateTime startTime { get; set;}
    public IEnumerable<TeamBracketExportViewModel> participants { get; set; }
    public int seedIndex { get; set; }
    public bool isLosers { get; set; }
}