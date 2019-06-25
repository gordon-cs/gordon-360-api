using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ScheduleViewModel
    {
    public string IDNumber { get; set; }


        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }


        public string BuildingCode { get; set; }

        public string RoomCode { get; set; }

        public string MonCode { get; set; }
        public string TueCode { get; set; }
        public string WedCode { get; set; }
        public string ThuCode { get; set; }
        public string FriCode { get; set; }

        public Nullable<System.DateTime> BeginTime { get; set; }

        public Nullable<System.DateTime> EndTime { get; set; }

    }


}