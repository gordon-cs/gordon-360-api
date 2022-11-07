using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantTeamViewModel
    {
        public int ID { get; set; }
        public int TeamID { get; set; }
        public int ParticipantID { get; set; }
        public DateTime SignDate { get; set; }
        public int RoleTypeID { get; set; }
    }
}
