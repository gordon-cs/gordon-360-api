using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{

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

        public DiningViewModel(IEnumerable<DiningInfoViewModel> diningTableEnumerable)
        {
            Swipes = new MealPlanComponent();
            DiningDollars = new MealPlanComponent();
            GuestSwipes = new MealPlanComponent();

            var diningTableList = diningTableEnumerable.ToList();

            if (diningTableList.Count == 0)
            {
                ChoiceDescription = "None";
            }
            else
            {
                ChoiceDescription = diningTableList[0].ChoiceDescription;

                foreach (DiningInfoViewModel m in diningTableEnumerable)
                {
                    if (m.PlanType.Equals(DOLLAR_TYPE))
                    {
                        DiningDollars.PlanId = m.PlanId;
                        DiningDollars.PlanDescription = m.PlanDescriptions;
                        DiningDollars.InitialBalance = m.InitialBalance;
                        DiningDollars.CurrentBalance = m.CurrentBalance;
                    }
                    else if (m.PlanDescriptions.ToLower().Contains("guest"))
                    {
                        GuestSwipes.PlanDescription = m.PlanDescriptions;
                        GuestSwipes.PlanId = m.PlanId;
                        GuestSwipes.InitialBalance = m.InitialBalance;
                        GuestSwipes.CurrentBalance = m.CurrentBalance;
                    }
                    else
                    {
                        Swipes.PlanDescription = m.PlanDescriptions;
                        Swipes.PlanId = m.PlanId;
                        Swipes.InitialBalance = m.InitialBalance;
                        Swipes.CurrentBalance = m.CurrentBalance;
                    }
                }
            }






        }
    }
}