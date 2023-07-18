using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels
{
    public class CoursesViewModel
    {
        public int IDNumber { get; set; }
        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public string CourseCode { get; set; }

        public string CourseTitle { get; set; }

        public string Location { get; set; }

        public string Day { get; set; }

    }

}