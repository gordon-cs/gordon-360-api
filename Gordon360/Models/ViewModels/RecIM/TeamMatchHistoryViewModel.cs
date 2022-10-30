using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamMatchHistoryViewModel
    {
        public TeamViewModel opponent { get; set; }
        public int OwnScore { get; set; }
        public int OpposingScore { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
        
    }
}