using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public record Schedule(
    bool Sun,
    bool Mon,
    bool Tue,
    bool Wed,
    bool Thu,
    bool Fri,
    bool Sat
 );
public class SeriesScheduleUploadViewModel
{
    public int? SeriesID { get; set; }
    public Schedule AvailableDays { get; set; }
    public IEnumerable<int> AvailableSurfaceIDs { get; set; }
    public DateTime DailyStartTime { get; set; }
    public DateTime DailyEndTime { get; set; }
    public int EstMatchTime { get; set; }
}