using System;

namespace Gordon360.Models.Salesforce;


public class AcademicTerm
{
    public enum Seasons { Fall, Spring, Summer, Winter }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Seasons CurrentSeason { get; set; }
    public bool IsActive { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public string ShowOnWeb { get; set; } = "";
    public AcademicYear AcademicYearID { get; set; } = new AcademicYear();
}
