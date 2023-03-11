using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchViewModel
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public int SurfaceID { get; set; }
        public int StatusID { get; set; }
        public int SeriesID { get; set; }
        public static implicit operator MatchViewModel(Match m)
        {
            return new MatchViewModel
            {
                ID = m.ID,
                StartTime = m.StartTime.SpecifyUtc(),
                SurfaceID = m.SurfaceID,
                StatusID = m.StatusID,
                SeriesID = m.SeriesID
            };
        }
    }
}