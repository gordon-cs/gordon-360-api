using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchCreatedViewModel
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public int SurfaceID { get; set; }
        public int StatusID { get; set; }
        public int SeriesID { get; set; }
        public static implicit operator MatchCreatedViewModel(Match m)
        {
            return new MatchCreatedViewModel
            {
                ID = m.ID,
                Time = m.Time,
                SurfaceID = m.SurfaceID,
                StatusID = m.StatusID,
                SeriesID = m.SeriesID
            };
        }
    }
}