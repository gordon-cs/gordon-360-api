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
        public string Description { get; set; }
        public int ActivityID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public ActivityViewModel Activity { get; set; }
        public virtual ICollection<MatchViewModel> Match { get; set; }
        //public virtual ICollection<SeriesTeamViewModel> SeriesTeam { get; set; }
    }
}