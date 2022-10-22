using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchViewModel
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public string Surface { get; set; }
        public string Status { get; set; }
        public virtual SeriesViewModel Series { get; set; }
        public virtual IEnumerable<MatchParticipant> MatchParticipant { get; set; }
        public virtual IEnumerable<TeamViewModel> Team { get; set; }
    }
}