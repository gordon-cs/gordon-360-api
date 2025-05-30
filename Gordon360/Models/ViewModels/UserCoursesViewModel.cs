using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class UserCoursesViewModel
{
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
    public DateTime? BEGIN_DATE { get; set; }
    public DateTime? END_DATE { get; set; }
    public string Role { get; set; }
    public static implicit operator UserCoursesViewModel(UserCourses course)
    {
        var code = course.YR_CDE;

        switch (course.TRM_CDE)
        {
            case "FA":
                code += "09";
                break;
            // We had to add a year to the YR_CDE because it was a year behind
            // compared to the academic system that we use on 360 (same for Summer courses).
            case "SP":
                code = (Int32.Parse(code) + 1).ToString() + "01";
                break;
            case "SU":
                code = (Int32.Parse(code) + 1).ToString() + "05";
                break;
            default:
                break;
        }

        UserCoursesViewModel vm = new UserCoursesViewModel
        {
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
            BEGIN_DATE = course.BEGIN_DATE,
            END_DATE = course.END_DATE,
            Role = course.Role
        };

        return vm;
    }
}