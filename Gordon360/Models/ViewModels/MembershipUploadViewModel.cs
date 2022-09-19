using System;

namespace Gordon360.Models.CCT
{
    public partial class MembershipUploadViewModel
    {
        public int MembershipID { get; set; }
        public string ACTCode { get; set; }
        public string SessCode { get; set; }
        public string email { get; set; }
        public int? GordonID { get; set; }
        public string PartCode { get; set; }
        public string CommentText { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? JobTime { get; set; }
        public bool? GroupAdmin { get; set; }
        public bool? Privacy { get; set; }
    }
}