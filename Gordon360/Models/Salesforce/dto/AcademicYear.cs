using System;

namespace Gordon360.Models.Salesforce;


public class AcademicYear
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Year { get; set; } = "";
    public bool IsLocked { get; set; } = false;
    public string ShowOnWeb { get; set; } = "";
}
