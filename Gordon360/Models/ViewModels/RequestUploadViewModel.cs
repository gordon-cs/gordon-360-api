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

        public static implicit operator REQUEST(RequestUploadViewModel request)
        {
            return new REQUEST
            {
                ACT_CDE = request.ACTCode,
                SESS_CDE = request.SessCode,
                PART_CDE = request.PartCode,
                DATE_SENT = request.DateSent,
                COMMENT_TXT = request.CommentText,
                STATUS = request.Status
            };
        }
    }
}