using System;

namespace Gordon360.Models.CCT
{
    public partial class MembershipUploadViewModel
    {
        public string ACTCode { get; set; }
        public string SessCode { get; set; }
        public string Username { get; set; }
        public string PartCode { get; set; }
        public string CommentText { get; set; }
        public bool GroupAdmin { get; set; }
        public bool Privacy { get; set; }
    }
}