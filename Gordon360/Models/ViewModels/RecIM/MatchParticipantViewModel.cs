using Gordon360.Models.Gordon360;

namespace Gordon360.Models.ViewModels.RecIM;

public class MatchParticipantViewModel
{
    public int ID { get; set; }
    public int MatchID { get; set; }
    public string ParticipantUsername { get; set; }
    public static implicit operator MatchParticipantViewModel(MatchParticipant m)
    {
        return new MatchParticipantViewModel
        {
            ID = m.ID,
            MatchID = m.MatchID,
            ParticipantUsername = m.ParticipantUsername
        };
    }
}