using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class CreateMatchViewModel
    {
        public DateTime Time { get; set; }
        public int SeriesID { get; set; }
        public int? SurfaceID { get; set; }
        public IEnumerable<int> TeamIDs { get; set; }
    }
}