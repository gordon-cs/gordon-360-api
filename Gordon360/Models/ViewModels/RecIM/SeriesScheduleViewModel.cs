using Gordon360.Models.CCT;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesScheduleViewModel
    {
        public int ID { get; set; }
        public Schedule AvailableDays { get; set; };

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EstMatchTime { get; set; }

        public static implicit operator SeriesScheduleViewModel(SeriesSchedule s)
        {
            //trial implementation
            return new SeriesScheduleViewModel
            {
                ID = s.ID,
                AvailableDays = new Schedule(s.Sun,s.Mon,s.Tue,s.Wed,s.Thu,s.Fri,s.Sat),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                EstMatchTime = s.EstMatchTime ?? 30 //customer suggestion default 30 minutes
            };
        }
    }
}