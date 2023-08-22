using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class SchedulerExtendedViewModel
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SportID { get; set; }
    public int? SurfaceID { get; set; }
    public IEnumerable<int> TeamIDs { get; set; }
}