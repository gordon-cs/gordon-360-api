using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class HallTaskViewModel
{
    public int Task_ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Hall_ID { get; set; }
    public bool Is_Recurring { get; set; }
    public string Frequency { get; set; }
    public int Interval { get; set; }
    public DateTime Start_Date { get; set; }
    public DateTime? End_Date { get; set; }
    public DateTime Created_Date { get; set; } = DateTime.Now;
    public DateTime? Completed_Date { get; set;}
    public string? Completed_By { get; set;}
    public DateTime? Occur_Date { get; set; }
}
