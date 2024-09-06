using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gordon360.Models.ViewModels;

public record ScheduleCourseViewModel
{
    public required string Code { get; set; }
    public required string YearCode { get; set; }
    public required string TermCode { get; set; }
    public required string? SubtermCode { get; set; }
    public required string Title { get; set; }
    public required string Role { get; set; }
    public string? Location { get; set; }
    public required List<char> MeetingDays { get; set; }
    public TimeOnly? BeginTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateOnly? BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }

    [SetsRequiredMembers]
    public ScheduleCourseViewModel(ScheduleCourse course)
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
        if (course.SUNDAY_CDE == "U")
        {
            meetingDays.Add('U');
        }

        Code = course.CRS_CDE;
        YearCode = course.YR_CDE;
        TermCode = course.TRM_CDE;
        SubtermCode = course.SUBTERM_CDE;
        Title = course.CRS_TITLE.Trim();
        Role = course.Role;
        Location = (course.BLDG_CDE, course.ROOM_CDE) switch
        {
            (string building_code, string room_code) => $"{building_code} {room_code}",
            (string building_code, null) => building_code,
            _ => null
        };
        MeetingDays = meetingDays;
        BeginTime = course.BeginTime;
        EndTime = course.EndTime;
        BeginDate = course.BeginDate;
        EndDate = course.EndDate;
    }
}
