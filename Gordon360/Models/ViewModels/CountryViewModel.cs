using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class CountryViewModel
    {
        public string CTY { get; set; }
        public string COUNTRY { get; set; }
        public static implicit operator CountryViewModel(Countries c)
        {
            CountryViewModel vm = new CountryViewModel
            {
                CTY = c.CTY,
                COUNTRY = c.COUNTRY,
            };
            return vm;
        }
    }
}
