using System;

namespace Gordon360.Models.ViewModels
{
    public class FoundActionsTakenViewModel
    {
        public int? ID { get; set; }
        public string foundID { get; set; }
        public string action { get; set; }
        public DateTime actionDate { get; set; }
        public string actionNote { get; set; }
        public string? submitterID { get; set; }
        public string submitterUsername { get; set; }

        public static implicit operator FoundActionsTakenViewModel(CCT.FoundActionsTakenData a) => new FoundActionsTakenViewModel
        {
            ID = a.ID,
            foundID = a.foundID,
            submitterUsername = a.AD_Username,
            action = a.action,
            actionDate = a.actionDate,
            actionNote = a.actionNote,
        };
    }
}
