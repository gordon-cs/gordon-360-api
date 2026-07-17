using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System;

namespace Gordon360.Models.ViewModels;

public class SessionViewModel
{
    public string SessionCode { get; set; }
    public string SessionDescription { get; set; }
    public DateTime? SessionBeginDate { get; set; }
    public DateTime? SessionEndDate { get; set; }

    public static implicit operator SessionViewModel(CM_SESSION_MSTR sess)
    {
        SessionViewModel vm = new()
        {

            SessionCode = sess.SESS_CDE.Trim(),
            SessionDescription = sess.SESS_DESC ?? "",
            SessionBeginDate = sess.WHEEL_BEGN_DTE ?? sess.SESS_BEGN_DTE ?? DateTime.MinValue,
            SessionEndDate = sess.WHEEL_END_DTE ?? sess.SESS_END_DTE ?? DateTime.MaxValue,

        };

        return vm;
    }

    public static explicit operator SessionViewModel(AcademicSession sess)
    {
        SessionViewModel vm = new()
        {

            SessionCode = sess.gc_Jenz_Session_Code__c,
            SessionDescription = sess.Name,
            SessionBeginDate = (sess.ClassStartDate is null) ? null : DateTime.Parse(sess.ClassStartDate, null, System.Globalization.DateTimeStyles.RoundtripKind),
            SessionEndDate = (sess.ExamEndDate is null) ? null : DateTime.Parse(sess.ExamEndDate, null, System.Globalization.DateTimeStyles.RoundtripKind)
        };

        return vm;
    }
}