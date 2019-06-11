using System;
using System.Globalization;
namespace Gordon360.Models.ViewModels
{
    public class StudentEmploymentViewModel
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
        public bool? Privacy { get; set; }
        public int AccountPrivate { get; set; }

        
        public static implicit operator StudentEmploymentViewModel(STUDENTEMPLOYMENT s)
        {
            StudentEmploymentViewModel vm = new StudentEmploymentViewModel
            {
                MembershipID = s.MEMBERSHIP_ID,
                ActivityCode = s.ACT_CDE.Trim(),
                SessionCode = s.SESS_CDE.Trim(),
                IDNumber = s.ID_NUM,
                Participation = s.PART_CDE.Trim(),
                GroupAdmin = s.GRP_ADMIN ?? false,
                StartDate = s.BEGIN_DTE, 
                EndDate = s.END_DTE,
                Description = s.COMMENT_TXT ?? "", // For Null comments
                Privacy = s.PRIVACY ?? false,
            };

            return vm;
        }
    }
}