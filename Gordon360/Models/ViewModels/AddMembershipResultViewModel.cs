using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class AddMembershipResultViewModel
    {
        public MEMBERSHIP membership { get; set; }
        public bool success { get; set; }
        public string? errorMessage { get; set; }

        public AddMembershipResultViewModel(MEMBERSHIP m, bool s, string? e = null)
        {
            membership = m;
            success = s;
            errorMessage = e;
        }
    }
}
