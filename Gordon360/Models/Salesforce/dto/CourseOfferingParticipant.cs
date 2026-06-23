namespace Gordon360.Models.Salesforce;




public class CourseOfferingParticipant
{
    public string ParticipantAffiliation { get; set; } = "";
    public string ParticipationStatus { get; set; } = "";
    

    public ParticipantContact ParticipantContact { get; set; } = new();
}