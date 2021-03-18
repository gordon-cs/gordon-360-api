namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment hall choice data to/from the frontend
    public class ApartmentChoiceViewModel
    {
        public int AprtAppID { get; set; }
        public int HallRank { get; set; }
        public string HallName { get; set; }
    }
}