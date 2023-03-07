using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchUploadViewModel
    {
        public DateTime StartTime { get; set; }
        public int SeriesID { get; set; }
        public int? SurfaceID { get; set; }
        public IEnumerable<int> TeamIDs { get; set; }
        public Match ToMatch()
        {
            return new Match
            {
                SeriesID = this.SeriesID,
                Time = this.StartTime,
                SurfaceID = this.SurfaceID ?? 1, //unknown surface id
                StatusID = 1 //default unconfirmed
            };
        }
    }
}