using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class CreateTeamViewModel
    {
        public string Name { get; set; }
        public int StatusID { get; set; }
        public int ActivityID { get; set; }
        public bool Private { get; set; }
        public string? Logo { get; set; }

    }
}
