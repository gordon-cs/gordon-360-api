using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class DailyTaskViewModel
{
    public int TaskID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HallID { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? OccurDate { get; set; }
}
