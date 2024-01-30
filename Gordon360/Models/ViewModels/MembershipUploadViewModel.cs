using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public partial class MembershipUploadViewModel
{
    public string Activity { get; set; }
    public string Session { get; set; }
    public string Username { get; set; }
    public string Participation { get; set; }
    public string CommentText { get; set; }
    public bool GroupAdmin { get; set; }
    public bool Privacy { get; set; }
    public MEMBERSHIP ToMembership(int gordonId, DateTime beginDate)
    {

        return new MEMBERSHIP()
        {
            ACT_CDE = this.Activity,
            SESS_CDE = this.Session,
            ID_NUM = gordonId,
            PART_CDE = this.Participation,
            BEGIN_DTE = beginDate,
            COMMENT_TXT = this.CommentText,
            GRP_ADMIN = this.GroupAdmin,
            PRIVACY = this.Privacy,
            USER_NAME = Environment.UserName,
            JOB_NAME = "360"
        };
    }

    public static MembershipUploadViewModel FromRequest(REQUEST request, String username)
    {
        return new MembershipUploadViewModel
        {
            Activity = request.ACT_CDE,
            Session = request.SESS_CDE,
            Username = username,
            Participation = request.PART_CDE,
            CommentText = request.COMMENT_TXT,
            GroupAdmin = false,
            Privacy = false
        };
    }

    public static implicit operator MembershipUploadViewModel(RequestUploadViewModel requestUpload)
    {

        return new MembershipUploadViewModel
        {
            Activity = requestUpload.Activity,
            Session = requestUpload.Session,
            Username = requestUpload.Username,
            Participation = requestUpload.Participation,
            CommentText = requestUpload.CommentText,
            GroupAdmin = false,
            Privacy = false
        };
    }
}
