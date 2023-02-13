using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesTeamPatchViewModel
    {
        public int ID { get; set; }
        public int? Win { get; set; }
        public int? Loss { get; set; }
        public int? Tie { get; set; }

    }
}