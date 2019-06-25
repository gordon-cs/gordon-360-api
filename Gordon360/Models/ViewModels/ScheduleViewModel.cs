using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class StudentScheduleViewModel
    {
    public int IDNumber {get; set;}
    public DateTime BeginTime { get; set; }

    public DateTime EndTime { get; set; }

    public string CourseCode { get; set; }

    public string CourseTitle { get; set; }

    public string Location { get; set; }

    public string Day { get; set; }

        public static implicit operator StudentScheduleViewModel(STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE_Result stu)
        {
            StudentScheduleViewModel vm = new StudentScheduleViewModel
            {
                IDNumber = stu.IDNumber,
                BeginTime = stu.BeginTime,
                EndTime = stu.EndTime,
                CourseCode = stu.CourseCode.Trim() ?? "",
                CourseTitle = stu.CourseTitle.Trim() ?? "",
                Location = stu.BuildingCode.Trim() + " "  + stu.RoomCode.Trim(),
                Day = stu.MonCode.Trim() ?? ""
                + stu.TueCode.Trim() ?? "" 
                + stu.WedCode.Trim() ?? "" 
                + stu.ThuCode.Trim() ?? "" 
                 +stu.FriCode.Trim() ?? "",
            };

            return vm;
        }
        

    }


}