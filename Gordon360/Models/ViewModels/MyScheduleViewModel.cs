using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class MyScheduleViewModel
    {
        public string EventID { get; set; }
        public string GordonID { get; set; } //should this really be a string? or an int?
        public string Location { get; set; }
        public string Description { get; set; }
        public string MonCode { get; set; }
        public string TueCode { get; set; }
        public string WedCode { get; set; }
        public string ThuCode { get; set; }
        public string FriCode { get; set; }
        public string SatCode { get; set; }
        public string SunCode { get; set; }
        public Nullable<int> IsAllDay { get; set; }
        public Nullable<System.TimeSpan> BeginTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }

        // Modeled more like EventViewModel, should this be more like the others that return vm?
        public MyScheduleViewModel(MYSCHEDULE sch)
        {
            EventID = sch.EVENT_ID;
            GordonID = sch.GORDON_ID;
            Location = sch.LOCATION ?? ""; // For Null locations;
            Description = sch.DESCRIPTION ?? ""; // For Null descriptions
            MonCode = sch.MON_CDE.Trim() ?? ""; // For Null days
            TueCode = sch.TUE_CDE.Trim() ?? ""; // For Null days
            WedCode = sch.WED_CDE.Trim() ?? ""; // For Null days
            ThuCode = sch.THU_CDE.Trim() ?? ""; // For Null days
            FriCode = sch.FRI_CDE.Trim() ?? ""; // For Null days
            SatCode = sch.SAT_CDE.Trim() ?? ""; // For Null days
            SunCode = sch.SUN_CDE.Trim() ?? ""; // For Null days
            IsAllDay = sch.IS_ALLDAY;
            BeginTime = sch.BEGIN_TIME;
            EndTime = sch.END_TIME;

        }
    }
}