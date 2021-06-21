using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class MembershipRequestViewModel
    {
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public int IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Participation { get; set; }
        public string ParticipationDescription { get; set; }
        public System.DateTime DateSent { get; set; }
        public int RequestID { get; set; }
        public string CommentText { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public string RequestApproved { get; set; }

        public static implicit operator MembershipRequestViewModel(REQUEST req)
        {
            MembershipRequestViewModel vm = new MembershipRequestViewModel
            {
                ActivityCode = req.ACT_CDE.Trim(),
                IDNumber = req.ID_NUM,
                Participation = req.PART_CDE.Trim(),
                DateSent = req.DATE_SENT,
                RequestID = req.REQUEST_ID,
                CommentText = req.COMMENT_TXT ?? "",// For null comments
                SessionCode = req.SESS_CDE.Trim(),
                RequestApproved = req.STATUS
            };

            return vm;
        }
    }


}