using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class ParticipantStatusExtendedViewModel
{
    public string Username { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}