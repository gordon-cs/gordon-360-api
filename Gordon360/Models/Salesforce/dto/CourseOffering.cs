namespace Gordon360.Models.Salesforce;


public class CourseOffering
{
    public string Name { get; set; } = "";


    public LearningCourse LearningCourse { get; set; } = new();
    public AcademicSession AcademicSession { get; set; } = new();
    

    public SalesforceChildCollection<CourseOfferingParticipant> CourseOfferingParticipants { get; set; } = new();

    public SalesforceChildCollection<CourseOfferingSchedule> CourseOfferingSchedules { get; set; } = new();
}