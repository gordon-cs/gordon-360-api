using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class TeamRecordViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Win { get; set; }  
        public int Loss { get; set; }
        public int Tie { get; set; }
    }
}