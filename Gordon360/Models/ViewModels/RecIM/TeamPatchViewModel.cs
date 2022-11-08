using System.Drawing.Text;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamPatchViewModel
    {
        public string Name { get; set; }
        public int? StatusID { get; set; }
        public bool Private { get; set; }
        public string? Logo { get; set; }
    }
}
