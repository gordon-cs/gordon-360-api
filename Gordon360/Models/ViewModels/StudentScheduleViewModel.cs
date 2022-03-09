using Gordon360.Models.CCT;
using System;

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

        public static implicit operator StudentScheduleViewModel(STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDEResult stu)
        {
            StudentScheduleViewModel vm = new StudentScheduleViewModel
            {
                IDNumber = stu.ID_NUM,
                BeginTime = (DateTime)(DateTime.Today + stu.BEGIN_TIME),
                EndTime = (DateTime)(DateTime.Today + stu.END_TIME),
                CourseCode = stu.CRS_CDE.Trim() ?? "",
                CourseTitle = stu.CRS_TITLE.Trim() ?? "",
                Location = stu.BLDG_CDE.Trim() + " "  + stu.ROOM_CDE.Trim(),
                Day = stu.MONDAY_CDE.Trim() ?? ""
                + stu.TUESDAY_CDE.Trim() ?? "" 
                + stu.WEDNESDAY_CDE.Trim() ?? "" 
                + stu.THURSDAY_CDE.Trim() ?? "" 
                 +stu.FRIDAY_CDE.Trim() ?? "",
            };

            return vm;
        }
        

    }


}