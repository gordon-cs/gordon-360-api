using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class CreateMatchViewModel
    {
        public DateTime Time { get; set; }
        public int SportID { get; set; }
        public int? SurfaceID { get; set; }
        public IEnumerable<Object> TeamIDs { get; set; }
    }
}