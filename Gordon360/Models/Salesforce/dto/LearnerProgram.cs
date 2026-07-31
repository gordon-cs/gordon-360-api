namespace Gordon360.Models.Salesforce;


public class LearnerProgram
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public LearningProgramPlan LearningProgramPlan { get; set; } = new ();
}
