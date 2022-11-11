using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ActivityID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public IEnumerable<MatchViewModel> Match { get; set; }
        public IEnumerable<TeamRecordViewModel> TeamStanding { get; set; }
    }
}