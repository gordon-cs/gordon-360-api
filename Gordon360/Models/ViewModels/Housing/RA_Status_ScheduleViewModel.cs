using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class RA_Status_ScheduleViewModel
{
    public int Sched_ID { get; set; } // ID for status ( e.g, "1", "2", etc )
    public string Ra_ID { get; set; } // ID of RA
    public string Status_name { get; set; } // Name of status ( e.g., "In Class" )
    public DateTime Start_time { get; set; } // Start time of a status ( e.g., 9:00 am )
    public DateTime End_time { get; set; } // End time of a status
    public bool Is_Recurring { get; set; } // Bool to determine if the status repeats
}
