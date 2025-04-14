using Gordon360.Models.CCT;
using System.Collections;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;
// TeamBracketExportViewModel is essentially a renamed version of TeamBracketExtendedViewModel with name 
// changes to mirror exactly that of the UI. As well as a type change from int -> string for tournamentRoundText
// This is purely for ease of access from the UI side and lighter calculations on the UI side.
public class TeamBracketExportViewModel
{
    public int id { get; set; }
    public string? resultText { get; set; }
    public bool isWinner { get; set; }
    public string? status { get; set; }
    public string? name { get; set; }
}