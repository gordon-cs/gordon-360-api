using Gordon360.Services;
using System;

namespace Gordon360.Models.CCT
{
    public partial class RequestUploadViewModel
    {
        public string ACTCode { get; set; }
        public string SessCode { get; set; }
        public string Username { get; set; }
        public string PartCode { get; set; }
        public DateTime DateSent { get; set; }
        public string CommentText { get; set; }
        public string Status { get; set; }

        public REQUEST ToREQUEST()
        {
            return new REQUEST
            {
                ACT_CDE = this.ACTCode,
                SESS_CDE = this.SessCode,
                PART_CDE = this.PartCode,
                DATE_SENT = this.DateSent,
                COMMENT_TXT = this.CommentText,
                STATUS = this.Status
            };
        }

        public MembershipUploadViewModel ToMembershipUpload()
        {
            return new MembershipUploadViewModel
            {
                ACTCode = this.ACTCode,
                SessCode = this.SessCode,
                Username = this.Username,
                PartCode = this.PartCode,
                CommentText = this.CommentText,
                GroupAdmin = false,
                Privacy = false
            };
        }
    }
}