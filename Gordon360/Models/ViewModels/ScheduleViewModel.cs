using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public int ScheduleID { get; set; }
        public int IDNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public string ItemDescription { get; set; }

        // Modeled more like EventViewModel, should this be more like the others that return vm?
        public ScheduleViewModel(SCHEDULE sch)
        {
            ScheduleID = sch.SCHD_ID;
            IDNumber = sch.ID_NUM;
            StartTime = sch.BEGIN_DTE;
            EndTime = sch.END_DTE;
            Title = sch.TITLE.Trim();
            ItemDescription = sch.DSCRPT_TXT.Trim() ?? ""; // For Null comments

        }
    }
}