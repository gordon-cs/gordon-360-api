using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class SeriesPatchViewModel
{
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? StatusID { get; set; }
    public int? ScheduleID { get; set; }
    public int[]? TeamIDs { get; set; }
    public int? WinnerID { get; set; }
    public int? Points { get; set; }
}