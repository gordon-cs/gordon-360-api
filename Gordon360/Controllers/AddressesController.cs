using Gordon360.Models.Gordon360;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class AddressesController(IAddressesService addressesService) : GordonControllerBase
{

    /// <summary>
    /// Pulls all states available from Jenzabar States Table
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("states")]
    public ActionResult<IEnumerable<States>> GetAllStates()
    {
        var result = addressesService.GetAllStates();

        return Ok(result);
    }

    /// <summary>
    /// Pulls all Countries available from Jenzabar Countries Table
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("countries")]
    public ActionResult<IEnumerable<CountryViewModel>> GetAllCountries()
    {
        var result = addressesService.GetAllCountries();

        return Ok(result);
    }
}
