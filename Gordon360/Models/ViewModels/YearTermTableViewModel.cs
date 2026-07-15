using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
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
       public YearTermTableViewModel(AcademicTerm entity)
        {
            YearCode = entity.gc_Jenz_Year_Code__c;
            TermCode = entity.gc_Jenz_Term_Code__c;
            // Convert ISO 8601 DateTime strings to DateTime objects
            BeginDate = DateTime.Parse(entity.StartDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
            EndDate = DateTime.Parse(entity.EndDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

            Description = entity.Name;

            // TODO: Field does not yet exist in SalesForce. Default placeholder value.
            ShowOnWeb = "y";
        }
    }
}
