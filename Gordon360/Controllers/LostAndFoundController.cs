using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.Controllers
{
    [Route("/api/[controller]")]
    public class LostAndFoundController(CCTContext context, ILostAndFoundService lostAndFoundService) : GordonControllerBase
    {
        [HttpGet]
        [Route("/MissingItems")]
        public ActionResult<IEnumerable<Missing>> GetMissingItems()
        {
            IEnumerable<Missing> result = lostAndFoundService.GetMissingItems();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/FoundItems")]
        public ActionResult<IEnumerable<FoundItems>> GetFoundItems()
        {
            IEnumerable<FoundItems> result = lostAndFoundService.GetFoundItems();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/FoundItemsByID")]
        public ActionResult<FoundItems> GetFoundItem(int itemID)
        {
            FoundItems? result = lostAndFoundService.GetFoundItem(itemID);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
