using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class RA_StatusEventsViewModel
{
    public int StatusID { get; set; }
    public string RaID { get; set; }
    public string StatusName { get; set; }
    public bool IsRecurring { get; set; }
    public string Frequency { get; set; }
    public int Interval { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? CompletedDate { get; set;}
    public DateTime? OccurDate { get; set; }
}
