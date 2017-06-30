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
        public Nullable<int> Required { get; set; }

        public static implicit operator ChapelEventViewModel(ChapelEvent a)
        {
            Nullable<DateTime> NoTIme = new DateTime();
            ChapelEventViewModel vm = new ChapelEventViewModel
            {

                ROWID = a.ROWID,
                CHBarEventID = a.CHBarEventID.Trim(),
                CHBarcode = a.CHBarcode.Trim(),
                CHEventID = a.CHEventID,
                CHCheckerID = a.CHCheckerID.Trim(),
                CHDate = a.CHDate ?? NoTIme,
                CHTime = a.CHTime,
                CHTermCD = a.CHTermCD.Trim(),
                Required = a.Required,
            };

            return vm;
        }
    }


}