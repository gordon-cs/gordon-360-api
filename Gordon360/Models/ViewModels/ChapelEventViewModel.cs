using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ChapelEventViewModel
    {
        public int ROWID { get; set; }
        public string CHBarEventID { get; set; }
        public string CHBarcode { get; set; }
        public string CHEventID { get; set; }
        public string CHCheckerID { get; set; }
        public Nullable<DateTime> CHDate { get; set; }
        public Nullable<DateTime> CHTime { get; set; }
        public string CHTermCD { get; set; }

        public static implicit operator ChapelEventViewModel(ChapelEvent a)
        {
            DateTime NOTIME = new DateTime();
            ChapelEventViewModel vm = new ChapelEventViewModel
            {
                ROWID = a.ROWID,
                CHBarEventID = a.CHBarEventID.Trim(),
                CHBarcode = a.CHBarcode.Trim(),
                CHEventID = a.CHEventID,
                CHCheckerID = a.CHCheckerID.Trim(),
                CHDate = a.CHDate ?? NOTIME,
                CHTime = a.CHTime ?? NOTIME,
                CHTermCD = a.CHTermCD.Trim(),
            };

            return vm;
        }
    }


}