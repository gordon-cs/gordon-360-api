using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class ActivityViewModel
    {
        public string ACT_CDE { get; set; }
        public string ACT_DESC { get; set; }

        public static implicit operator ActivityViewModel(ACT_INFO a)
        {
            ActivityViewModel vm = new ActivityViewModel
            {
                ACT_CDE = a.ACT_CDE.Trim(),
                ACT_DESC = a.ACT_DESC.Trim()
            };

            return vm;
        }
    }
}