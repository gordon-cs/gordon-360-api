using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public record CountryViewModel(string Name, string Abbreviation)
    {
        public static implicit operator CountryViewModel(Countries c) => new(Name: c.COUNTRY, Abbreviation: c.CTY);
    }
}
