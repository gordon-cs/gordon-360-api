using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;
public class RD_On_Call_Create
{
    public int RD_ID { get; set; }
    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }
    public DateTime Created_Date { get; set; }

}
