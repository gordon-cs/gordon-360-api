using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly CCTContext _context;

        public AddressesService(CCTContext context)
        {
            _context = context;
        }

        public List<States> GetAllStates()
        {

            return _context.States.ToList();
        }

        public List<Countries> GetAllCountries()
        {
            return _context.Countries.ToList();
        }
    }
}
