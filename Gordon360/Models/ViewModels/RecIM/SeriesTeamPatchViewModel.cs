using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesTeamPatchViewModel
    {
        public int ID { get; set; }
        public int? WinCount { get; set; }
        public int? LossCount { get; set; }
        public int? TieCount { get; set; }

    }
}