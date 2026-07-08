using System;

namespace Gordon360.Models.Salesforce;


public class PersonEmployment
{
    public string Position { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Now;
}
