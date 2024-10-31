using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Gordon360.Controllers
{
    [Route("/api/[controller]")]
    public class LostAndFoundController(CCTContext context, ILostAndFoundService lostAndFoundService) : GordonControllerBase
    {
        [HttpPost]
        [Route("missingitem")]
        public ActionResult<int> CreateMissingItemReport([FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            int ID = lostAndFoundService.CreateMissingItemReport(MissingItemDetails);

            return Ok(ID);
        }

        /// <summary>
        /// Update Missing Item Report with the given id
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns></returns>
        [HttpPut]
        [Route("missingitem/id")]
        public async Task<ActionResult> UpdateMissingItemReport(int id, [FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            await lostAndFoundService.UpdateMissingItemReportAsync(id, MissingItemDetails);

            return Ok();
        }

        /// <summary>
        /// Update Item Report Status to "Deleted" with the given id
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns></returns>
        [HttpPut]
        [Route("missingitem/status")]
        public async Task<ActionResult> UpdateReportStatus(int id, [FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            await lostAndFoundService.UpdateReportStatusAsync(id, MissingItemDetails);

            return Ok();
        }


        [HttpGet]
        [Route("missingitems")]
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
        [Route("founditems")]
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
        [Route("founditemsbyid")]
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

        /// <param name="id">The id</param>
        [HttpGet]
        [Route("missingitemsbyid")]
        public ActionResult<Missing> GetMissingItem(int id)
        {
            Missing? result = lostAndFoundService.GetMissingItem(id);
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
