using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ActivityID { get; set; }
        public int TypeID { get; set; }
        public int StatusID { get; set; }
        public int? ScheduleID { get; set; }

        public static implicit operator SeriesViewModel(Series s)
        {
            return new SeriesViewModel
            {
                ID = s.ID,
                Name = s.Name,
                StartDate = s.StartDate.SpecifyUtc(),
                EndDate = s.EndDate.SpecifyUtc(),
                ActivityID = s.ActivityID,
                TypeID = s.TypeID,
                StatusID = s.StatusID,
                ScheduleID = s.ScheduleID
            };
        }
    }

    
}