using Gordon360.Models.CCT;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesSchedulePatchViewModel
    {
        public IEnumerable<ScheduleDayViewModel> AvailableDays { get; set; }
        public DateTime? DailyStartTime { get; set; }
        public DateTime? DailyEndTime { get; set; }
        public int? EstMatchTime { get; set; }
    }
}