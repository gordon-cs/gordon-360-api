using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM;


// Mirror of Match Bracket DB Model

public class MatchBracketViewModel
{
    public int MatchID { get; set; }
    public int RoundNumber { get; set; }
    public int RoundOf { get; set; }
    public int SeedIndex { get; set; }
    public bool IsLosers { get; set; }
    public DateTime StartTime { get; set; }


    public static implicit operator MatchBracketViewModel(MatchBracket m)
    {
        return new MatchBracketViewModel
        {
            MatchID = m.MatchID,
            RoundNumber = m.RoundNumber,
            RoundOf = m.RoundOf,
            SeedIndex = m.SeedIndex,
            IsLosers = m.IsLosers
        };
    }

}