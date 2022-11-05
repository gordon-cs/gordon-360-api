using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SchedulerViewModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime SeriesStartDate { get; set; }
        public DateTime SeriesEndDate { get; set; }
        public int EstimatedActivityTime { get; set; }
        public IEnumerable<int>? AvailableSurfaceIDs { get; set; }
        public IEnumerable<int>? TeamIDs { get; set; }
    }
}