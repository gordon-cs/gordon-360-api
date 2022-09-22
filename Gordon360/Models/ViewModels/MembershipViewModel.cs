using Gordon360.Models.CCT;
using System;
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
        public int? IDNumber { get; set; }
        public string AD_Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail_Location { get; set; }
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
    }
}