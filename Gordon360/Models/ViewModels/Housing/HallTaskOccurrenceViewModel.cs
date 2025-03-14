using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class HallTaskOccurrenceViewModel
{
    public int OccurID { get; set; }
    public int TaskID { get; set; }
    public DateTime OccurDate { get; set; }
    public bool IsComplete { get; set; }
    public string CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
}

