using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class ParticipantStatusHistoryViewModel
{
    public int ID { get; set; }
    public string ParticipantUsername { get; set; }
    public int StatusID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public static implicit operator ParticipantStatusHistoryViewModel(ParticipantStatusHistory s)
    {
        return new ParticipantStatusHistoryViewModel
        {
            ID = s.ID,
            ParticipantUsername = s.ParticipantUsername,
            StatusID = s.StatusID,
            StartDate = s.StartDate.SpecifyUtc(),
            EndDate = s.EndDate.SpecifyUtc(),
        };
    }
}