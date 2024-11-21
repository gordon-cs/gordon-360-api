﻿using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class LostAndFoundController(CCTContext context, ILostAndFoundService lostAndFoundService) : GordonControllerBase
    {

        /// <summary>
        /// Create a new missing item report with given data
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>ObjectResult(ID) - An HTTP result code, with the ID of the created report if created successfully</returns>
        [HttpPost]
        [Route("missingitem")]
        public ActionResult<int> CreateMissingItemReport([FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            int ID = lostAndFoundService.CreateMissingItemReport(MissingItemDetails, authenticatedUserUsername);

            return Ok(ID);
        }

        /// <summary>
        /// Create action taken of the missing item report with actions taken object and missing item report ID
        /// </summary>
        /// <param name="id">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPost]
        [Route("missingitem/{id}/actionTaken")]
        public ActionResult<int> CreateActionTaken(int id, [FromBody] ActionsTakenViewModel ActionsTaken)
        {
            int ID = lostAndFoundService.CreateActionTaken(id, ActionsTaken);

            return Ok(ID);
        }

        /// <summary>
        /// Update Missing Item Report with the given id with given data
        /// </summary>
        /// <param name="id">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitem/{id}")]
        public async Task<ActionResult> UpdateMissingItemReport(int id, [FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            await lostAndFoundService.UpdateMissingItemReportAsync(id, MissingItemDetails);

            return Ok();
        }

        /// <summary>
        /// Update the status of the item report with given id to the given status text
        /// </summary>
        /// <param name="id">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitem/{id}/{status}")]
        public async Task<ActionResult> UpdateReportStatus(int id, string status)
        {
            await lostAndFoundService.UpdateReportStatusAsync(id, status);

            return Ok();
        }

        /// <summary>
        /// Get all missing items, only for data entry level users
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("missingitemsall")]
        [StateYourBusiness(operation = Static.Names.Operation.READ_ALL, resource = Resource.LOST_AND_FOUND_MISSING_REPORT)]
        public ActionResult<IEnumerable<MissingItemReportViewModel>> GetAllMissingItems()
        {
            IEnumerable<MissingItemReportViewModel> result = lostAndFoundService.GetMissingItemsAll();
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
        [Route("missingitems")]
        public ActionResult<IEnumerable<MissingItemReportViewModel>> GetMissingItems()
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            IEnumerable<MissingItemReportViewModel> result = lostAndFoundService.GetMissingItems(authenticatedUserUsername);
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
        [Route("missingitemsbyid/{id}")]
        public ActionResult<MissingItemReportViewModel> GetMissingItem(int id)
        {
            MissingItemReportViewModel? result = lostAndFoundService.GetMissingItem(id);
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
        [Route(("missingitem/{id}/actionsTakenAll"))]
        public ActionResult<IEnumerable<ActionsTakenViewModel>> GetActionsTaken(int id)
        {
            IEnumerable<ActionsTakenViewModel> result = lostAndFoundService.GetActionsTaken(id);
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
