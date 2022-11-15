
using Gordon360.Models.ViewModels;

namespace Gordon360.Services
{
    public class MembershipUploadResultViewModel
    {
        public MembershipUploadViewModel Membership { get; set; }
        public string? TextResult { get; set; }
        public bool Success { get; set; }
    }
}