using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;

public record ScheduleCourseViewModel
{
    public string Code { get; set; }
    public string Title { get; set; }
    public string Role { get; set; }
    public string? Location { get; set; }
    public List<char> MeetingDays { get; set; }
    public TimeOnly? BeginTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateOnly? BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string YearTermCode { get; set; }


    public static implicit operator ScheduleCourseViewModel(UserCourses course)
    {
        List<char> meetingDays = new();
        if (course.MONDAY_CDE == "M")
        {
            meetingDays.Add('M');
        }
        if (course.TUESDAY_CDE == "T")
        {
            meetingDays.Add('T');
        }
        if (course.WEDNESDAY_CDE == "W")
        {
            meetingDays.Add('W');
        }
        if (course.THURSDAY_CDE == "R")
        {
            meetingDays.Add('R');
        }
        if (course.FRIDAY_CDE == "F")
        {
            meetingDays.Add('F');
        }
        if (course.SATURDAY_CDE == "S")
        {
            meetingDays.Add('S');
        }
        if (course.MONDAY_CDE == "U")
        {
            meetingDays.Add('U');
        }

        return new ScheduleCourseViewModel()
        {
            Code = course.CRS_CDE.Trim(),
            Title = course.CRS_TITLE.Trim(),
            Role = course.Role,
            Location = (course.BLDG_CDE, course.ROOM_CDE) switch
            {
                (string building_code, string room_code) => $"{building_code} {room_code}",
                (string building_code, null) => building_code,
                _ => null
            },
            MeetingDays = meetingDays,
            BeginTime = course.BEGIN_TIME is TimeSpan BeginTime ? TimeOnly.FromTimeSpan(BeginTime) : null,
            EndTime = course.END_TIME is TimeSpan EndTime ? TimeOnly.FromTimeSpan(EndTime) : null,
            BeginDate = course.BEGIN_DATE is DateTime BeginDate ? DateOnly.FromDateTime(BeginDate) : null,
            EndDate = course.END_DATE is DateTime EndDate ? DateOnly.FromDateTime(EndDate) : null,
            YearTermCode = course.YR_CDE + course.TRM_CDE,
        };
    }
}