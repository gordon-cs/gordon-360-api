using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
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
        /// Update Missing Item Report with the given id with given data
        /// </summary>
        /// <param name="missingItemId">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action, with the ID of the action taken</returns>
        [HttpPost]
        [Route("missingitem/{missingItemId}/actiontaken")]
        public ActionResult<int> CreateActionTaken(int missingItemId, [FromBody] ActionsTakenViewModel ActionsTaken)
        {
            int actionId = lostAndFoundService.CreateActionTaken(missingItemId, ActionsTaken);

            return Ok(actionId);
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
        /// <param name="status"></param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitem/{id}/{status}")]
        public async Task<ActionResult> UpdateReportStatus(int id, string status)
        {
            await lostAndFoundService.UpdateReportStatusAsync(id, status);

            return Ok();
        }

        /// <summary>
        /// Get all missing item reports, only for data entry level users
        /// </summary>
        /// <returns>ObjectResult - an http status code, with an array of MissingItem objects in the body </returns>
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

        /// <summary>
        /// Get the list of missing item reports for the currently authenticated user.
        /// </summary>
        /// <returns>ObjectResult - an http status code, with an array of MissingItem objects in the body </returns>
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

        /// <summary>
        /// Get a missing item report with given ID.
        /// </summary>
        /// <param name="id">The id of the report to get</param>
        /// <returns>ObjectResult - an http status code, with a MissingItem object in the body </returns>
        [HttpGet]
        [Route("missingitemsbyid/{id}")]
        public ActionResult<MissingItemReportViewModel> GetMissingItem(int id)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            MissingItemReportViewModel? result = lostAndFoundService.GetMissingItem(id, authenticatedUserUsername);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get all actions taken on a given missing item report.
        /// </summary>
        /// <param name="id">The id of the report to get</param>
        /// <returns>ObjectResult - an http status code, with a list of Actions Taken objects </returns>
        [HttpGet]
        [Route(("missingitem/{id}/actionstakenall"))]
        public ActionResult<IEnumerable<ActionsTakenViewModel>> GetActionsTaken(int id)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            IEnumerable<ActionsTakenViewModel> result = lostAndFoundService.GetActionsTaken(id, authenticatedUserUsername);
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
