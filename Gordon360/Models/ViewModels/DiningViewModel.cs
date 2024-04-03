using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels;




public class MealPlanComponent
{
    public string PlanDescription { get; set; }
    public string PlanId { get; set; }
    public string PlanType { get; set; }

    public int InitialBalance { get; set; }

    public string CurrentBalance { get; set; }
}

public class DiningViewModel
{
    private const string SWIPE_TYPE = "MEALS";
    private const string DOLLAR_TYPE = "DCB";

    public string ChoiceDescription { get; set; }
    public MealPlanComponent Swipes { get; set; }
    public MealPlanComponent DiningDollars { get; set; }
    public MealPlanComponent GuestSwipes { get; set; }

    public DiningViewModel(IEnumerable<DiningTableViewModel> diningTableEnumerable)
    {
        this.Swipes = new MealPlanComponent();
        this.DiningDollars = new MealPlanComponent();
        this.GuestSwipes = new MealPlanComponent();

        var diningTableList = diningTableEnumerable.ToList();

        if (diningTableList.Count == 0)
        {
            this.ChoiceDescription = "None";
        }
        else
        {
            this.ChoiceDescription = diningTableList[0].ChoiceDescription;

            foreach (DiningTableViewModel m in diningTableEnumerable)
            {
                if (m.PlanType.Equals(DOLLAR_TYPE))
                {
                    this.DiningDollars.PlanId = m.PlanId;
                    this.DiningDollars.PlanDescription = m.PlanDescriptions;
                    this.DiningDollars.InitialBalance = m.InitialBalance;
                    this.DiningDollars.CurrentBalance = m.CurrentBalance;
                }
                else if (m.PlanDescriptions.ToLower().Contains("guest"))
                {
                    this.GuestSwipes.PlanDescription = m.PlanDescriptions;
                    this.GuestSwipes.PlanId = m.PlanId;
                    this.GuestSwipes.InitialBalance = m.InitialBalance;
                    this.GuestSwipes.CurrentBalance = m.CurrentBalance;
                }
                else
                {
                    this.Swipes.PlanDescription = m.PlanDescriptions;
                    this.Swipes.PlanId = m.PlanId;
                    this.Swipes.InitialBalance = m.InitialBalance;
                    this.Swipes.CurrentBalance = m.CurrentBalance;
                }
            }
        }






    }
}