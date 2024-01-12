using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gordon360.Models.ViewModels;

public partial record ScheduleCourseViewModel
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
    public string YearCode { get; set; }
    public string TermCode { get; set; }
    public string? SubTermCode { get; set; }


    public static implicit operator ScheduleCourseViewModel(ScheduleCourses course)
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
            Code = RepeatedSpacesRegex().Replace(course.CRS_CDE.Trim(), " "),
            Title = course.CRS_TITLE.Trim(),
            Role = course.Role,
            Location = (course.BLDG_CDE, course.ROOM_CDE) switch
            {
                (string building_code, string room_code) => $"{building_code} {room_code}",
                (string building_code, null) => building_code,
                _ => null
            },
            MeetingDays = meetingDays,
            BeginTime = course.BeginTime is TimeSpan BeginTime ? TimeOnly.FromTimeSpan(BeginTime) : null,
            EndTime = course.EndTime is TimeSpan EndTime ? TimeOnly.FromTimeSpan(EndTime) : null,
            BeginDate = course.BeginDate is DateTime BeginDate ? DateOnly.FromDateTime(BeginDate) : null,
            EndDate = course.EndDate is DateTime EndDate ? DateOnly.FromDateTime(EndDate) : null,
            YearCode = course.YR_CDE,
            TermCode = course.TRM_CDE,
            SubTermCode = course.SUBTERM_CDE,
        };
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex RepeatedSpacesRegex();
}
