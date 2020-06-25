using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ShiftToSubmitViewModel
    {
        public int ID_NUM { get; set; }
        public int EML { get; set; }
        public DateTime SHIFT_END_DATETIME { get; set; }
        public int SUBMITTED_TO { get; set; }
        public string LAST_CHANGED_BY { get; set; }
    }
}