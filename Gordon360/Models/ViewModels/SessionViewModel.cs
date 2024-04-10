using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class SessionViewModel
{
    public string SessionCode { get; set; }
    public string SessionDescription { get; set; }
    public Nullable<System.DateTime> SessionBeginDate { get; set; }
    public Nullable<System.DateTime> SessionEndDate { get; set; }

    public static implicit operator SessionViewModel(CM_SESSION_MSTR sess)
    {
        SessionViewModel vm = new SessionViewModel
        {

            SessionCode = sess.SESS_CDE.Trim(),
            SessionDescription = sess.SESS_DESC ?? "",
            SessionBeginDate = sess.WHEEL_BEGN_DTE ?? sess.SESS_BEGN_DTE ?? DateTime.MinValue,
            SessionEndDate = sess.WHEEL_END_DTE ?? sess.SESS_END_DTE ?? DateTime.MaxValue,

        };

        return vm;
    }
}