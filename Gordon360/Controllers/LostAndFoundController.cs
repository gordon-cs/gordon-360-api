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
        [Route("missingitems")]
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
        [Route("missingitems/{missingItemId}/actionstaken")]
        public ActionResult<int> CreateActionTaken(int missingItemId, [FromBody] ActionsTakenViewModel ActionsTaken)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            int actionId = lostAndFoundService.CreateActionTaken(missingItemId, ActionsTaken, authenticatedUserUsername);

            return Ok(actionId);
        }

        /// <summary>
        /// Update Missing Item Report with the given id with given data
        /// </summary>
        /// <param name="missingItemId">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitems/{missingItemId}")]
        public async Task<ActionResult> UpdateMissingItemReport(int missingItemId, [FromBody] MissingItemReportViewModel MissingItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateMissingItemReportAsync(missingItemId, MissingItemDetails, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Update the status of the item report with given id to the given status text
        /// </summary>
        /// <param name="missingItemId">The id of the report to update</param>
        /// <param name="status"></param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitems/{missingItemId}/{status}")]
        public async Task<ActionResult> UpdateReportStatus(int missingItemId, string status)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateReportStatusAsync(missingItemId, status, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Get the list of missing item reports for the currently authenticated user.
        /// </summary>
        /// <param name="color">The selected color for filtering reports</param>
        /// <param name="category">The selected category for filtering reports</param>
        /// <param name="keywords">The selected keywords for filtering by keywords</param>
        /// <param name="status">The selected status for filtering reports</param>
        /// <param name="user">Query parameter, default is null and route will get all missing items, or if user is set
        /// route will get missing items for the authenticated user</param>
        /// <param name="lastId">The ID of the last fetched report to start from for pagination</param>
        /// <param name="pageSize">The size of the page to fetch for pagination</param>
        /// <returns>ObjectResult - an http status code, with an array of MissingItem objects in the body </returns>
        [HttpGet]
        [Route("missingitems")]
        public ActionResult<IEnumerable<MissingItemReportViewModel>> GetMissingItems(string? user = null,
                                                                                     int? lastId = null,
                                                                                     int? pageSize = null,
                                                                                     string? status = null,
                                                                                     string? color = null,
                                                                                     string? category = null,
                                                                                     string? keywords = null)
        {
            IEnumerable<MissingItemReportViewModel> result;
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            // If no username specified in the query, get all items
            if (user == null)
            {
                result = lostAndFoundService.GetMissingItemsAll(authenticatedUserUsername, lastId, pageSize, status, color, category, keywords);
            }
            else
            {
                result = lostAndFoundService.GetMissingItems(user, authenticatedUserUsername);
            }

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
        /// <param name="missingItemId">The id of the report to get</param>
        /// <returns>ObjectResult - an http status code, with a MissingItem object in the body </returns>
        [HttpGet]
        [Route("missingitems/{missingItemId}")]
        public ActionResult<MissingItemReportViewModel> GetMissingItem(int missingItemId)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            MissingItemReportViewModel? result = lostAndFoundService.GetMissingItem(missingItemId, authenticatedUserUsername);
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
        /// <param name="missingItemId">The id of the report to get</param>
        /// <returns>ObjectResult - an http status code, with a list of Actions Taken objects </returns>
        [HttpGet]
        [Route(("missingitems/{missingItemId}/actionstaken"))]
        public ActionResult<IEnumerable<ActionsTakenViewModel>> GetActionsTaken(int missingItemId)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            IEnumerable<ActionsTakenViewModel> result = lostAndFoundService.GetActionsTaken(missingItemId, authenticatedUserUsername);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route(("founditems"))]
        public ActionResult<string> CreateFoundItemReport([FromBody] FoundItemViewModel FoundItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            string reportID = lostAndFoundService.CreateFoundItem(FoundItemDetails, authenticatedUserUsername);

            return Ok(reportID);
        }

        /// <summary>
        /// Update Found Item with the given id with given data
        /// </summary>
        /// <param name="itemId">The id of the report to update</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route(("founditems/{itemId}"))]
        public async Task<ActionResult> UpdateFoundItem(string itemId, [FromBody] FoundItemViewModel FoundItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateFoundItemReportAsync(itemId, FoundItemDetails, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Update the status of the found item report with given id to the given status text
        /// </summary>
        /// <param name="itemId">The id of the report to update</param>
        /// <param name="status"></param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("founditems/{itemId}/{status}")]
        public async Task<ActionResult> UpdateFoundStatus(string itemId, string status)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateFoundStatusAsync(itemId, status, authenticatedUserUsername);

            return Ok();
        }
    }
}
