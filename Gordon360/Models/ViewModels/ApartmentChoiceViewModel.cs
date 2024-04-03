using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels;

// The view model used to send/receive apartment hall choice data to/from the frontend
public class ApartmentChoiceViewModel
{
    public int? ApplicationID { get; set; }
    public int HallRank { get; set; }
    public string HallName { get; set; }

    public static implicit operator ApartmentChoiceViewModel(Housing_HallChoices apartmentChoiceDBModel) => new ApartmentChoiceViewModel
    {
        ApplicationID = apartmentChoiceDBModel.HousingAppID,
        HallRank = apartmentChoiceDBModel.Ranking,
        HallName = apartmentChoiceDBModel.HallName,
    };
}
