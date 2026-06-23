namespace Gordon360.Models.Salesforce;



public class CourseOfferingSchedule
{
    public string Description { get; set; } = "";

    public bool IsSunday { get; set; }
    public bool IsMonday { get; set; }
    public bool IsTuesday { get; set; }
    public bool IsWednesday { get; set; }
    public bool IsThursday { get; set; }
    public bool IsFriday { get; set; }
    public bool IsSaturday { get; set; }

    public Location Location { get; set; } = new();

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
}