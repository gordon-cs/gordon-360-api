using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class DailyTaskViewModel
{
    public int Task_ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Hall_ID { get; set; }
    public DateTime? Completed_Date { get; set; }
    public string? Completed_By { get; set; }
    public DateTime? Occur_Date { get; set; }
}
