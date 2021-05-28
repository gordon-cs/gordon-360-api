namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment hall choice data to/from the frontend
    public class ApartmentChoiceViewModel
    {
        public int? ApplicationID { get; set; }
        public int HallRank { get; set; }
        public string HallName { get; set; }

        public static implicit operator ApartmentChoiceViewModel(GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result apartmentChoiceDBModel)
        {
            ApartmentChoiceViewModel vm = new ApartmentChoiceViewModel
            {
                ApplicationID = apartmentChoiceDBModel.AprtAppID,
                HallRank = apartmentChoiceDBModel.Ranking,
                HallName = apartmentChoiceDBModel.HallName,
            };

            return vm;
        }
    }
}
