using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantAttendanceViewModel
    {
        public IEnumerable<IndividualAttendance> Attendance { get; set; }

    }

    public class IndividualAttendance
    {
        public int MatchID { get; set; }
        public string Username { get; set; }
    }
}