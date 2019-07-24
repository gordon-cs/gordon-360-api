using System;
using System.Globalization;
namespace Gordon360.Models.ViewModels
{
    public class MembershipViewModel
    {
        
        public int MembershipID { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityImage { get; set; }
        public string ActivityImagePath { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public int IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Participation { get; set; }
        public string ParticipationDescription { get; set; }
        public bool? GroupAdmin { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Description { get; set; }
        public string ActivityType { get; set; }
        public string ActivityTypeDescription { get; set; }
        public bool? Privacy { get; set; }
        public int AccountPrivate { get; set; }

        
        public static implicit operator MembershipViewModel(MEMBERSHIP m)
        {
            MembershipViewModel vm = new MembershipViewModel
            {
                MembershipID = m.MEMBERSHIP_ID,
                ActivityCode = m.ACT_CDE.Trim(),
                SessionCode = m.SESS_CDE.Trim(),
                IDNumber = m.ID_NUM,
                Participation = m.PART_CDE.Trim(),
                GroupAdmin = m.GRP_ADMIN ?? false,
                StartDate = m.BEGIN_DTE, 
                EndDate = m.END_DTE,
                Description = m.COMMENT_TXT ?? "", // For Null comments
                Privacy = m.PRIVACY ?? false,
            };

            return vm;
        }
    }
}