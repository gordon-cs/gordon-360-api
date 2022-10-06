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
        public static MembershipUploadViewModel FromREQUEST(REQUEST request)
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
    }

}