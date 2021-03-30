namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment applicant data to/from the frontend
    public class ApartmentApplicantViewModel
    {
        public int ApplicationID { get; set; }
        public string Username { get; set; }
        public int? Age { get; set; }
        public string Class { get; set; }
        public string OffCampusProgram { get; set; }
        public bool Probation { get; set; }
        public int Points { get; set; }
    }
}