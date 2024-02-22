using Gordon360.Models.Gordon360;

namespace Gordon360.Models.ViewModels.RecIM;

public class ParticipantActivityViewModel
{
    public int ID { get; set; }
    public int ActivityID { get; set; }
    public string ParticipantUsername { get; set; }
    public int PrivTypeID { get; set; }
    public bool IsFreeAgent { get; set; }

    public static implicit operator ParticipantActivityViewModel(ParticipantActivity p)
    {
        return new ParticipantActivityViewModel
        {
            ID = p.ID,
            ActivityID = p.ActivityID,
            ParticipantUsername = p.ParticipantUsername,
            PrivTypeID = p.PrivTypeID,
            IsFreeAgent = p.IsFreeAgent
        };
    }
}