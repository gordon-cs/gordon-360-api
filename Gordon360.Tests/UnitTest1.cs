

public class CountryViewModelTests
{
    [Fact]
    public void ImplicitConversion_CreatesCorrectViewModel()
    {
        var country = new Countries
        {
            COUNTRY = "Brazil",
            CTY = "BR"
        };

        CountryViewModel result = country;

        Assert.Equal("Brazil", result.Name);
        Assert.Equal("BR", result.Abbreviation);
    }
}

