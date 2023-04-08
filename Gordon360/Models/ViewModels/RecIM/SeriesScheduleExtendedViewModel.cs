using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesScheduleExtendedViewModel
    {
        public int ID { get; set; }
        public Dictionary<string, bool> AvailableDays { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EstMatchTime { get; set; }
        public IEnumerable<int> TeamIDs { get; set; }

        public static implicit operator SeriesScheduleExtendedViewModel(SeriesSchedule s)
        {
            return new SeriesScheduleExtendedViewModel
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
                StartTime = s.StartTime.SpecifyUtc(),
                EndTime = s.EndTime.SpecifyUtc(),
                EstMatchTime = s.EstMatchTime ?? 30 //customer suggestion default 30 minutes
            };
        }
    }
}