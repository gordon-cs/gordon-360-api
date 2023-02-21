using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantAttendanceViewModel
    {
        public IEnumerable<Individual> Attendance { get; set; }
        public int TeamID { get; set; }

    }

    public class Individual
    {
        public int? ID { get; set; }
        public int? MatchID { get; set; }
        public string Username { get; set; }
        public int? TeamID { get; set; }

        public static implicit operator Individual(MatchParticipant mp)
        {
            Individual vm = new Individual
            {
                ID = mp.ID,
                MatchID = mp.MatchID,
                Username = mp.ParticipantUsername,
                TeamID = mp.TeamID,
            };

            return vm;
        }

    }
}