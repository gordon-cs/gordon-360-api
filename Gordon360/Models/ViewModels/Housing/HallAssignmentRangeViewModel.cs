using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.Housing
{
    public class HallAssignmentRangeViewModel
    {
        public string Hall_ID { get; set; }      // ID for the hall (e.g., "Chase")
        public string Room_Start { get; set; }   // Start room number (e.g., "101")
        public string Room_End { get; set; }     // End room number (e.g., "120")
    }
}
