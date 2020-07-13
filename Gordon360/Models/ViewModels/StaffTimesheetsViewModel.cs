using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class StaffTimesheetsViewModel
    {
        public int ID_NUM { get; set; }
        public int EML { get; set; }
        public string EML_DESCRIPTION { get; set; }
        public System.DateTime SHIFT_START_DATETIME { get; set; }
        public System.DateTime SHIFT_END_DATETIME { get; set; }
        public decimal HOURLY_RATE { get; set; }
        public decimal HOURS_WORKED { get; set; }
        public int SUPERVISOR { get; set; }
        public int COMP_SUPERVISOR { get; set; }
        public string STATUS { get; set; }
        public Nullable<int> SUBMITTED_TO { get; set; }
        public string SHIFT_NOTES { get; set; }
        public string COMMENTS { get; set; }
        public Nullable<System.DateTime> PAY_WEEK_DATE { get; set; }
        public Nullable<System.DateTime> PAY_PERIOD_DATE { get; set; }
        public Nullable<int> PAY_PERIOD_ID { get; set; }
        public string LAST_CHANGED_BY { get; set; }
        public System.DateTime DATETIME_ENTERED { get; set; }
    }
}