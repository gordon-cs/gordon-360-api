using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using Gordon360.Extensions.System;


namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesExtendedViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ActivityID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public IEnumerable<MatchExtendedViewModel> Match { get; set; }
        public IEnumerable<TeamRecordViewModel> TeamStanding { get; set; }
        public SeriesScheduleViewModel? Schedule { get; set; }

        public static implicit operator SeriesExtendedViewModel(Series s)
        {
            return new SeriesExtendedViewModel
            {
                ID = s.ID,
                Name = s.Name,
                StartDate = s.StartDate.SpecifyUtc(),
                EndDate = s.EndDate.SpecifyUtc(),
                ActivityID = s.ActivityID,
                Type = s.Type.Description,
                Status = s.Status.Description,
                Schedule = s.Schedule,
            };
        }
    }
}