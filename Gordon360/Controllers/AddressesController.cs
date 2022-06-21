using Gordon360.AuthorizationFilters;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressesService _addressesController;

        public AddressesController(IAddressesService addressesController)
        {
            _addressesController = addressesController;
        }

        /// <summary>
        /// Pulls all states available from Jenzabar States Table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("states")]
        public ActionResult<List<States>> GetAllStates()
        {
            var result = _addressesController.GetAllStates();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Pulls all Countries available from Jenzabar Countries Table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("countries")]
        public ActionResult<List<Countries>> GetAllCountries()
        {
            var result = _addressesController.GetAllCountries();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
