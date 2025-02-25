using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class DailyStatusEventsViewModel
{
    public int StatusID { get; set; }
    public string RaID { get; set; }
    public string StatusName { get; set; }
    public TimeSpan Start_Time { get; set; }
    public TimeSpan End_Time { get; set;}

}
