using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels
{
    public class FacultyScheduleViewModel
    {
    public int IDNumber {get; set;}
    public DateTime BeginTime { get; set; }

    public DateTime EndTime { get; set; }

    public string CourseCode { get; set; }

    public string CourseTitle { get; set; }

    public string Location { get; set; }

    public string Day { get; set; }

        public static implicit operator FacultyScheduleViewModel(INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDEResult fac)
        {
            FacultyScheduleViewModel vm = new FacultyScheduleViewModel
            {
                IDNumber = fac.ID_NUM.GetValueOrDefault(),
                BeginTime = (DateTime)(DateTime.Today + fac.BEGIN_TIME),
                EndTime = (DateTime)(DateTime.Today + fac.END_TIME),
                CourseCode = fac.CRS_CDE.Trim() ?? "",
                CourseTitle = fac.CRS_TITLE.Trim() ?? "",
                Location = fac.BLDG_CDE.Trim() + " "  + fac.ROOM_CDE.Trim(),
                Day = fac.MONDAY_CDE.Trim() ?? ""
                + fac.TUESDAY_CDE.Trim() ?? "" 
                + fac.WEDNESDAY_CDE.Trim() ?? "" 
                + fac.THURSDAY_CDE.Trim() ?? "" 
                 + fac.FRIDAY_CDE.Trim() ?? "",
            };

            return vm;
        }
        
    }

}