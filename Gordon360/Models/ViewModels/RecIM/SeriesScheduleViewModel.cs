using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesScheduleViewModel
    {
        public int ID { get; set; }
        public Dictionary<string,bool> AvailableDays { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EstMatchTime { get; set; }

        public static implicit operator SeriesScheduleViewModel(SeriesSchedule s)
        {
            return new SeriesScheduleViewModel
            {
                ID = s.ID,
                AvailableDays = new Dictionary<string, bool>
                {
                    { "Sunday",s.Sun },
                    { "Monday",s.Mon },
                    { "Tuesday",s.Tue },
                    { "Wednesday",s.Wed },
                    { "Thursday",s.Thu },
                    { "Friday",s.Fri },
                    { "Saturday",s.Sat },
                },
                StartTime = Helpers.FormatDateTimeToUtc(s.StartTime),
                EndTime = Helpers.FormatDateTimeToUtc(s.EndTime),
                EstMatchTime = s.EstMatchTime ?? 30 //customer suggestion default 30 minutes
            };
        }
    }
}