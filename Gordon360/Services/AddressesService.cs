using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services;

public class AddressesService(CCTContext context) : IAddressesService
{
    public IEnumerable<States> GetAllStates() => context.States.AsEnumerable();

    public IEnumerable<CountryViewModel> GetAllCountries() => context.Countries.Select<Countries, CountryViewModel>(c => c);
}