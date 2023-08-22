using Gordon360.Models.CCT;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class ParticipantAttendanceViewModel
{
    public IEnumerable<MatchAttendance> Attendance { get; set; }
    public int TeamID { get; set; }

}

public class MatchAttendance
{
    public int? ID { get; set; }
    public int? MatchID { get; set; }
    public string Username { get; set; }
    public int? TeamID { get; set; }

    public static implicit operator MatchAttendance(MatchParticipant mp)
    {
        return new MatchAttendance
        {
            ID = mp.ID,
            MatchID = mp.MatchID,
            Username = mp.ParticipantUsername,
            TeamID = mp.TeamID,
        };
    }

}