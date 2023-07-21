using Gordon360.Models.CCT;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Graph;
using System;
using System.Runtime.Serialization.Formatters;

namespace Gordon360.Models.ViewModels
{
    public class UserCoursesViewModel
    {
        public string UserID { get; set; }
        public int STUDENT_ID { get; set; }
        public int? INSTRUCTOR_ID { get; set; }
        public string SessionCode { get; set; }
        public string CRS_CDE { get; set; }
        public string CRS_TITLE { get; set; }
        public string BLDG_CDE { get; set; }
        public string ROOM_CDE { get; set; }
        public string MONDAY_CDE { get; set; }
        public string TUESDAY_CDE { get; set; }
        public string WEDNESDAY_CDE { get; set; }
        public string THURSDAY_CDE { get; set; }
        public string FRIDAY_CDE { get; set; }
        public string SATURDAY_CDE { get; set; }
        public TimeSpan? BEGIN_TIME { get; set; }
        public TimeSpan? END_TIME { get; set; }
        public string Role { get; set; }
        public static implicit operator UserCoursesViewModel(UserCourses course)
        {
            var code = course.YR_CDE;
            if(course.TRM_CDE == "FA")
            {
                code += "09";
            } else if (course.TRM_CDE == "SP")
            {
                code += "01";
            } else
            {
                code += "05";
            }

            UserCoursesViewModel vm = new UserCoursesViewModel
            {
                UserID =course.UserID,
                STUDENT_ID = course.STUDENT_ID,
                INSTRUCTOR_ID = course.INSTRUCTOR_ID,
                SessionCode = code,
                CRS_CDE = course.CRS_CDE,
                CRS_TITLE = course.CRS_TITLE,
                BLDG_CDE = course.BLDG_CDE,
                ROOM_CDE = course.ROOM_CDE,
                MONDAY_CDE = course.MONDAY_CDE,
                TUESDAY_CDE = course.TUESDAY_CDE,
                WEDNESDAY_CDE = course.WEDNESDAY_CDE,
                THURSDAY_CDE = course.THURSDAY_CDE,
                FRIDAY_CDE = course.FRIDAY_CDE,
                SATURDAY_CDE = course.SATURDAY_CDE,
                BEGIN_TIME = course.BEGIN_TIME,
                END_TIME = course.END_TIME,
                Role = course.Role
            };

            return vm;
        }
    }


}