using Gordon360.Services;
using System;

namespace Gordon360.Models.CCT
{
    public partial class RequestUploadViewModel
    {
        public string Activity { get; set; }
        public string Session { get; set; }
        public string Username { get; set; }
        public string Participation { get; set; }
        public DateTime DateSent { get; set; }
        public string CommentText { get; set; }
        public string Status { get; set; }

        public static implicit operator REQUEST(RequestUploadViewModel request)
        {
            return new REQUEST
            {
                ACT_CDE = request.Activity,
                SESS_CDE = request.Session,
                PART_CDE = request.Participation,
                DATE_SENT = request.DateSent,
                COMMENT_TXT = request.CommentText,
                STATUS = request.Status
            };
        }
    }
}