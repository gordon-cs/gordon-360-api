using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantTeamViewModel
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public string ParticipantUsername { get; set; }
        public DateTime? SignDate { get; set; }
        public int RoleTypeID { get; set; }

        public static implicit operator ParticipantTeamViewModel(ParticipantTeam pt)
        {
            return new ParticipantTeamViewModel
            {
                ID = pt.ID,
                TeamID = pt.TeamID,
                ParticipantUsername = pt.ParticipantUsername,
                SignDate = pt.SignDate.SpecifyUtc(),
                RoleTypeID = pt.RoleTypeID,
            };
        }
    }
}
