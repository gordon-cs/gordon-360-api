using Gordon360.Models.Gordon360;

namespace Gordon360.Models.ViewModels;

public record CountryViewModel(string Name, string Abbreviation)
{
    public static implicit operator CountryViewModel(Countries c) => new(Name: c.COUNTRY, Abbreviation: c.CTY);
}