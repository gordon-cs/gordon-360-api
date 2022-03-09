namespace Gordon360.Models.ViewModels
{
    public class AdvisorViewModel
    {
        public AdvisorViewModel(string fname, string lname, string adname)
        {
            Firstname = fname;
            Lastname = lname;
            ADUserName = adname;
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ADUserName { get; set; }
    }
}