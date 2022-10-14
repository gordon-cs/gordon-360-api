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
        public MEMBERSHIP ToMembership(int gordonId, DateTime beginDate)
        {

            return new MEMBERSHIP()
            {
                ACT_CDE = this.ACTCode,
                SESS_CDE = this.SessCode,
                ID_NUM = gordonId,
                BEGIN_DTE = beginDate,
                PART_CDE = this.PartCode,
                COMMENT_TXT = this.CommentText,
                GRP_ADMIN = this.GroupAdmin,
                PRIVACY = this.Privacy,
                USER_NAME = Environment.UserName
            };
        }

        public static implicit operator MembershipUploadViewModel(REQUEST request)
        {
            return new MembershipUploadViewModel
            {
                ACTCode = request.ACT_CDE,
                SessCode = request.SESS_CDE,
                PartCode = request.PART_CDE,
                CommentText = request.COMMENT_TXT,
                GroupAdmin = false,
                Privacy = false
            };
        }

        public static implicit operator MembershipUploadViewModel(RequestUploadViewModel requestUpload)
        {

            return new MembershipUploadViewModel
            {
                ACTCode = requestUpload.ACTCode,
                SessCode = requestUpload.SessCode,
                Username = requestUpload.Username,
                PartCode = requestUpload.PartCode,
                CommentText = requestUpload.CommentText,
                GroupAdmin = false,
                Privacy = false
            };
        }
    }

}