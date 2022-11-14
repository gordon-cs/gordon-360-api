using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly CCTContext _context;

        public AddressesService(CCTContext context)
        {
            _context = context;
        }

        public IEnumerable<States> GetAllStates() => _context.States;

        public IEnumerable<CountryViewModel> GetAllCountries() => _context.Countries.Select<Countries, CountryViewModel>(c => c);
    }
}
