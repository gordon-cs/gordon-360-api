using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ScheduleViewModel
    {
    public int ID_NUM {get; set;}
    public string CRS_CDE {get; set;}
    public string CRS_TITLE {get; set;}
    
    public string BLDG_CDE { get; set; }

    public int ROOM_CDE { get; set; }

    public string MONDAY_CDE { get; set; }
    public string TUESDAY_CDE { get; set; }
    public string WEDNESDAY_CDE { get; set; }
    public string THURSDAY_CDE { get; set; }
    public string FRIDAY_CDE { get; set; }

    public Nullable<System.DateTime> BEGIN_TIME { get; set; }

    public Nullable<System.DateTime> END_TIME { get; set; }


    }


}