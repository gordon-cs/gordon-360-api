using System.Globalization;
namespace CCT_App.Models.ViewModels
{
    public class MembershipViewModel
    {
        
        public int MembershipID { get; set; }
        public string ActivityCode { get; set; }
        public string SessionCode { get; set; }
        public string IDNumber { get; set; }
        public string Participation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }

        public static implicit operator MembershipViewModel(Membership m)
        {
            string newEndDate = "";
            if (m.END_DTE.HasValue)
            {
                newEndDate = m.END_DTE.Value.ToString("D", CultureInfo.CurrentCulture);
            }
            MembershipViewModel vm = new MembershipViewModel
            {
                MembershipID = m.MEMBERSHIP_ID,
                ActivityCode = m.ACT_CDE,
                SessionCode = m.SESSION_CDE,
                IDNumber = m.ID_NUM,
                Participation = m.PART_LVL,
                StartDate = m.BEGIN_DTE.ToString("D", CultureInfo.CurrentCulture),
                EndDate = newEndDate,
                Description = m.DESCRIPTION ?? ""
            };

            return vm;
        }
    }
}