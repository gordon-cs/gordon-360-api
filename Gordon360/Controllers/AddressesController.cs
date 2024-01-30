using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class AddressesController : GordonControllerBase
{
    private readonly IAddressesService _addressesService;

    public AddressesController(IAddressesService addressesService)
    {
        _addressesService = addressesService;
    }

    /// <summary>
    /// Pulls all states available from Jenzabar States Table
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("states")]
    public ActionResult<IEnumerable<States>> GetAllStates()
    {
        var result = _addressesService.GetAllStates();

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
        var result = _addressesService.GetAllCountries();

        return Ok(result);
    }
}
