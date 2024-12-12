using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.Housing
{
    public class HallAssignmentRangeViewModel
    {
        public int RangeID { get; set; }         //ID of room range
        public string Hall_ID { get; set; }      // ID for the hall (e.g., "Chase")
        public int Room_Start { get; set; }   // Start room number (e.g., 101)
        public int Room_End { get; set; }     // End room number (e.g., 120)
        public string? Assigned_RA {  get; set; } //The ID of the assigned RA
    }
}
