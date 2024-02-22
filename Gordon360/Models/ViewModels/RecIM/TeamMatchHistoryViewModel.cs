using Gordon360.Extensions.System;
using Gordon360.Models.Gordon360;
using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class TeamMatchHistoryViewModel
{
    public int TeamID { get; set; }
    public int MatchID { get; set; }
    public TeamExtendedViewModel Opponent { get; set; }
    public int TeamScore { get; set; }
    public int OpposingTeamScore { get; set; }
    public string Status { get; set; }
    public int MatchStatusID { get; set; }
    public DateTime MatchStartTime { get; set; }
    public int? SportsmanshipScore { get; set; }
    public static implicit operator TeamMatchHistoryViewModel(MatchTeam mt)
    {
        return new TeamMatchHistoryViewModel
        {
            TeamID = mt.TeamID,
            MatchID = mt.MatchID,
            TeamScore = mt.Score,
            Status = mt.Status.Description,
            MatchStatusID = mt.Match.StatusID,
            MatchStartTime = mt.Match.StartTime.SpecifyUtc(),
            SportsmanshipScore = mt.SportsmanshipScore
        };
    }
}