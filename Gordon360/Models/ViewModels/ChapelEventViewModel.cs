using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class ChapelEventViewModel
    {
        public string LiveID { get; set; }
        public int? Required { get; set; }
        public DateTime? CHDate { get; set; }
        public DateTime? CHTime { get; set; }
        public string CHTermCD { get; set; }

        public static implicit operator ChapelEventViewModel(ChapelEvent a)
        {
            ChapelEventViewModel vm = new ChapelEventViewModel
            {
                LiveID = a.LiveID.ToString().Trim(),
                Required = a.Required,
                CHDate = a.CHDate,
                CHTime = a.CHTime,
                CHTermCD = a.CHTermCD.Trim()
            };

            return vm;
        }
    }


}