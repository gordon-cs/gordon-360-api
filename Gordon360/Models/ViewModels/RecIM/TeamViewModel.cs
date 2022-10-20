using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        //private may be removed if deemed unecessary by customer
        public bool Private { get; set; }
        public string Logo { get; set; }
        public virtual ICollection<MatchViewModel> Matches { get; set; }
        public virtual ICollection<ParticipantViewModel> Participants { get; set; }
        //public virtual ICollection<SeriesTeamViewModel> SeriesTeam { get; set; }

    }
}