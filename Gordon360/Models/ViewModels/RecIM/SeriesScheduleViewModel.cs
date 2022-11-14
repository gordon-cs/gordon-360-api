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
        public Dictionary<string,bool> AvailableDays { get; set; } = new Dictionary<string,bool>();
        //public bool Sun { get; set; }
        //public bool Mon { get; set; }
        //public bool Tue { get; set; }
        //public bool Wed { get; set; }
        //public bool Thu { get; set; }
        //public bool Fri { get; set; }
        //public bool Sat { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EstMatchTime { get; set; }

        public static implicit operator SeriesScheduleViewModel(SeriesSchedule s)
        {
            //trial implementation
            return new SeriesScheduleViewModel
            {
                ID = s.ID,
                AvailableDays = new Dictionary<string, bool>()
                    {
                        { "Sunday",s.Sun },
                        { "Monday",s.Mon },
                        { "Tuesday",s.Tue },
                        { "Wednesday",s.Wed },
                        { "Thursday",s.Thu },
                        { "Friday",s.Fri },
                        { "Saturday",s.Sat },
                    },
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                EstMatchTime = s.EstMatchTime ?? 30 //customer suggestion default 30 minutes
            };
            //return new SeriesScheduleViewModel
            //{
            //    ID = s.ID,
            //    Sun = s.Sun,
            //    Mon = s.Mon,
            //    Tue = s.Tue,
            //    Wed = s.Wed,
            //    Thu = s.Thu,
            //    Fri = s.Fri,
            //    Sat = s.Sat,
            //    StartTime = s.StartTime,
            //    EndTime = s.EndTime,
            //    EstMatchTime = s.EstMatchTime
            //};
        }
    }
}