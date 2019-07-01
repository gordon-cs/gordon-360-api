using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public static implicit operator FacultyScheduleViewModel(INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE_Result fac)
        {
            FacultyScheduleViewModel vm = new FacultyScheduleViewModel
            {
                IDNumber = fac.IDNumber,
                BeginTime = fac.BeginTime,
                EndTime = fac.EndTime,
                CourseCode = fac.CourseCode.Trim() ?? "",
                CourseTitle = fac.CourseTitle.Trim() ?? "",
                Location = fac.BuildingCode.Trim() + " "  + fac.RoomCode.Trim(),
                Day = fac.MonCode.Trim() ?? ""
                + fac.TueCode.Trim() ?? "" 
                + fac.WedCode.Trim() ?? "" 
                + fac.ThuCode.Trim() ?? "" 
                 + fac.FriCode.Trim() ?? "",
            };

            return vm;
        }
        

    }


}