using System;
using System.Globalization;
namespace Gordon360.Models.ViewModels
{
    public class StudentEmploymentViewModel
    {
        
        public string Job_Title { get; set; }
        public string Job_Department { get; set; }
        public string Job_Department_Name { get; set; }
        public DateTime Job_Start_Date { get; set; }
        public Nullable<DateTime> Job_End_Date { get; set; }
        public DateTime Job_Expected_End_Date { get; set; }

        
        public static implicit operator StudentEmploymentViewModel(STUDENTEMPLOYMENT s)
        {
            StudentEmploymentViewModel vm = new StudentEmploymentViewModel
            {
                Job_Title = s.Job_Title.Trim(),
                Job_Department = s.Job_Department.Trim(),
                Job_Department_Name = s.Job_Department_Name.Trim(),
                Job_Start_Date = s.Job_Start_Date,
                Job_End_Date = s.Job_End_Date,
                Job_Expected_End_Date = s.Job_Expected_End_Date
            };

            return vm;
        }
    }
}