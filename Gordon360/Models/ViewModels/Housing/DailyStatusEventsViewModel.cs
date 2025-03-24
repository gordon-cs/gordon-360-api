using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class DailyStatusEventsViewModel
{
    public int Status_ID { get; set; }
    public string RA_ID { get; set; }
    public string Status_Name { get; set; }
    public TimeSpan Start_Time { get; set; }
    public TimeSpan End_Time { get; set;}
    public DateTime Start_Date { get; set; }
    public DateTime? End_Date { get; set; }
    public bool Available { get; set; }

}
