using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public class UserCoursesViewModel
{
    public string SessionCode { get; set; }
    public string YR_CDE { get; set; }
    public string TRM_CDE { get; set; }
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
    public string SUB_TERM_CDE { get; set; }
    public string Role { get; set; }

    public static implicit operator UserCoursesViewModel(UserCourses course)
    {
        var code = FormatSessionCode(course.YR_CDE, course.TRM_CDE);

        var subterm = FormatSubtermCode( course.SUBTERM_DESC);

        UserCoursesViewModel vm = new()
        {
            SessionCode = code,
            YR_CDE = course.YR_CDE,
            TRM_CDE = course.TRM_CDE,
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
            SUB_TERM_CDE = subterm,
            Role = course.Role
        };

        return vm;
    }

    public static UserCoursesViewModel FromCourseOffering(CourseOffering course)
    {
        var yearCode = course.AcademicSession?.gc_Jenz_Year_Code__c ?? "";
        var termCode = course.AcademicSession?.gc_Jenz_Term_Code__c ?? "";
        var code = FormatSessionCode(yearCode, termCode);

        var subtermDesc = course.AcademicSession?.gc_Jenz_Subterm_Code__c ?? "";
        var subterm = FormatSubtermCode(subtermDesc);

        var schedule = course.CourseOfferingSchedules?.records?.FirstOrDefault();
        var participant = course.CourseOfferingParticipants?.records?.FirstOrDefault();


        return new UserCoursesViewModel
        {
            SessionCode = code,
            YR_CDE = course.AcademicSession?.gc_Jenz_Year_Code__c ?? "",
            TRM_CDE = course.AcademicSession?.gc_Jenz_Term_Code__c ?? "",
            CRS_CDE = $"{course.LearningCourse?.SubjectAbbreviation}-{course.LearningCourse?.CourseNumber}",
            CRS_TITLE = course.Name ?? "",
            BLDG_CDE = schedule?.Location?.ExternalReference ?? "",
            // ROOM_CDE = course.ROOM_CDE, // TODO: Implement Room Code
            MONDAY_CDE = DayCode(schedule?.IsMonday, "M"),
            TUESDAY_CDE = DayCode(schedule?.IsTuesday, "T"),
            WEDNESDAY_CDE = DayCode(schedule?.IsWednesday, "W"),
            THURSDAY_CDE = DayCode(schedule?.IsThursday, "R"),
            FRIDAY_CDE = DayCode(schedule?.IsFriday, "F"),
            SATURDAY_CDE = DayCode(schedule?.IsSaturday, "S"),
            BEGIN_TIME = ParseTime(schedule?.StartTime),
            END_TIME = ParseTime(schedule?.EndTime),
            BEGIN_DATE = schedule?.StartDate,
            END_DATE = schedule?.EndDate,
            SUB_TERM_CDE = subterm,
            Role = participant?.ParticipantAffiliation ?? "",
        };
    }

    private static string FormatSessionCode(string yearCode, string termCode)
    {
        var code = yearCode;

        switch (termCode)
        {
            case "FA":
                code += "09";
                break;
            // We had to add a year to the YearCode because it was a year behind
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
        return code;
    }
    private static string FormatSubtermCode(string subtermDesc)
    {
        var subterm = subtermDesc;

        switch (subtermDesc)
        {
            case "1Q":
                subterm = "Fall 1";
                break;
            case "2Q":
                subterm = "Fall 2";
                break;
            case "3Q":
                subterm = "Spring 1";
                break;
            case "4Q":
                subterm = "Spring 2";
                break;
            case "5Q":
                subterm = "Summer 1";
                break;
            case "6Q":
                subterm = "Summer 2";
                break;
            default:
                break;

        }

        return subterm;
    }
    private static string DayCode(bool? flag, string code) => flag == true ? code : "";
    private static TimeSpan? ParseTime(string? time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return null;
        }
        else
        {
            var cleanedTime = time.Replace("Z", "");
            var isValid = TimeSpan.TryParse(cleanedTime, out var t);

            return isValid ? t : null;
        }
    }
}