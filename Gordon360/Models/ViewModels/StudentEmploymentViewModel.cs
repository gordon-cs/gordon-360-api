using System;
namespace Gordon360.Models.ViewModels
{
    public class StudentEmploymentViewModel
    {

        public string Job_Title { get; set; }
        public string Job_Department { get; set; }
        public string Job_Department_Name { get; set; }
        public Nullable<DateTime> Job_Start_Date { get; set; }
        public Nullable<DateTime> Job_End_Date { get; set; }
        public Nullable<DateTime> Job_Expected_End_Date { get; set; }

    }
}