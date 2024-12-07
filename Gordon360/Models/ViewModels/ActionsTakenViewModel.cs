using System;

namespace Gordon360.Models.ViewModels
{
    public class ActionsTakenViewModel
    {
        public int? ID { get; set; }
        public int missingID { get; set; }
        public string action { get; set; }
        public DateTime actionDate { get; set; }
        public string actionNote { get; set; }
        public string? submitterID { get; set; }
        public string username { get; set; }
        public bool isPublic { get; set; }

        public static implicit operator ActionsTakenViewModel(CCT.ActionsTakenData a) => new ActionsTakenViewModel
        {
            ID = a.ID,
            missingID = a.missingID,
            username = a.AD_Username,
            action = a.action,
            actionDate = a.actionDate,
            actionNote = a.actionNote,
            isPublic = a.isPublic,
        };
    }
}
