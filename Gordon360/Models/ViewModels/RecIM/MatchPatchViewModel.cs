using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class MatchPatchViewModel
    {
        public DateTime? Time { get; set; }
        public int? SurfaceID { get; set; }
        public int? StatusID { get; set; }
        public IEnumerable<int>? TeamIDs { get; set; }
    }
}