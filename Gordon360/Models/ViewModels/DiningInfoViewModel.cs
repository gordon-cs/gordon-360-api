namespace Gordon360.Models.ViewModels
{
    public record DiningInfoViewModel(string ChoiceDescription,
                                      string PlanDescriptions,
                                      string PlanId,
                                      string PlanType,
                                      int InitialBalance,
                                      string CurrentBalance);
}