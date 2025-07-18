using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels
{
    public class YearTermTableViewModel
    {
        public string YearCode { get; set; }
        public string TermCode { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string ShowOnWeb { get; set; }

        public YearTermTableViewModel(YearTermTable entity)
        {
            YearCode = entity.YR_CDE;
            TermCode = entity.TRM_CDE;
            BeginDate = entity.TRM_BEGIN_DTE;
            EndDate = entity.TRM_END_DTE;
            Description = entity.YR_TRM_DESC;
            ShowOnWeb = entity.SHOW_ON_WEB;
        }
    }
}
