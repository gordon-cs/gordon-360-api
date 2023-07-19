using Gordon360.Models.CCT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gordon360.Models.ViewModels
{
    public class SessionCoursesViewModel
    {
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public Nullable<System.DateTime> SessionBeginDate { get; set; }
        public Nullable<System.DateTime> SessionEndDate { get; set; }
        public IEnumerable <ScheduleViewModel> AllCourses { get; set; }
    }


}