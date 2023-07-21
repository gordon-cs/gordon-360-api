using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesPatchViewModel
    {
        public string? Name { get; set; }
        public TimeOnly? StartDate { get; set; }
        public TimeOnly? EndDate { get; set; }
        public int? StatusID { get; set; }
        public int? ScheduleID { get; set; }
        public int[]? TeamIDs { get; set; }
    }
}