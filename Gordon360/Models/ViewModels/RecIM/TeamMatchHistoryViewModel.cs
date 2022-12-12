using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamMatchHistoryViewModel
    {
        public int OwnID { get; set; }
        public int MatchID { get; set; }
        public TeamViewModel Opponent { get; set; }
        public int OwnScore { get; set; }
        public int OpposingScore { get; set; }
        public string Status { get; set; }
        public int MatchStatusID { get; set; }
        public DateTime Time { get; set; }
        
    }
}