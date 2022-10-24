using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchTeamViewModel
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public int MatchID { get; set; }
        public int StatusID { get; set; }
        public int Score { get; set; }
        public int? Sportmanship { get; set; }

        public static implicit operator MatchTeamViewModel(MatchTeam m)
        {
            var vm = new MatchTeamViewModel
            {
                ID = m.ID,
                TeamID = m.TeamID,
                MatchID = m.MatchID,
                StatusID = m.StatusID,
                Score = m.Score,
                Sportmanship = m.Sportsmanship
            };
            return vm;
        }

    }
}