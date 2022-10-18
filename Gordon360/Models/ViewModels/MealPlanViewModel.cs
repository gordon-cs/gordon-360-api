using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public record MealPlanComponent(string ChoiceDescription,
                                      string PlanDescriptions,
                                      string PlanType,
                                      int InitialBalance,
                                      string CurrentBalance);

    public record MealPlanViewModel
    {
        public string ChoiceDescription { get; init; }
        public IEnumerable<MealPlanComponent> MealPlanComponents { get; init; }

        public MealPlanViewModel(IEnumerable<MealPlanComponent> mealPlanComponents)
        {
            ChoiceDescription = mealPlanComponents.FirstOrDefault()?.ChoiceDescription ?? "None"; 
            MealPlanComponents = mealPlanComponents;
        }
    }
}