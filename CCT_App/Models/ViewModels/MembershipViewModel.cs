using System;
using System.Globalization;
namespace CCT_App.Models.ViewModels
{
    public class MembershipViewModel
    {
        
        public int MembershipID { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityImage { get; set; }
        public string ActivityMeetingTime { get; set; }
        public string ActivityMeetingday { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Participation { get; set; }
        public string ParticipationDescription { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Description { get; set; }

        
        public static implicit operator MembershipViewModel(Membership m)
        {
            MembershipViewModel vm = new MembershipViewModel
            {
                MembershipID = m.MEMBERSHIP_ID,
                ActivityCode = m.ACT_CDE.Trim(),
                SessionCode = m.SESSION_CDE.Trim(),
                IDNumber = m.ID_NUM.Trim(),
                Participation = m.PART_LVL.Trim(),
                StartDate = m.BEGIN_DTE, 
                EndDate = m.END_DTE,
                Description = m.DESCRIPTION ?? "" // For Null descriptions
            };

            return vm;
        }
    }
}