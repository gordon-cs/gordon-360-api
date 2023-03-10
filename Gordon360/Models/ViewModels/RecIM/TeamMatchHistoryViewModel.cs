using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
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
            var m = mt;
            return new TeamMatchHistoryViewModel
            {
                TeamID = mt.TeamID,
                MatchID = mt.MatchID,
                TeamScore = mt.Score,
                Status = mt.Status.Description,
                MatchStatusID = mt.Match.StatusID,
                MatchStartTime = mt.Match.StartTime,
                SportsmanshipScore = mt.SportsmanshipScore
            };
        }
    }
}