using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
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

        public IEnumerable<States> GetAllStates()
        {

            return _context.States.AsEnumerable();
        }

        public IEnumerable<Countries> GetAllCountries()
        {
            return _context.Countries.AsEnumerable();
        }
    }
}
