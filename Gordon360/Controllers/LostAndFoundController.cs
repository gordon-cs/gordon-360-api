using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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
        /// Update the status of the item report with given id to the given status text
        /// </summary>
        /// <param name="missingItemId">The id of the report to update</param>
        /// <param name="foundItemID"></param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("missingitems/{missingItemId}/linkItem/{foundItemID?}")]
        public async Task<ActionResult> UpdateReportAssociatedFoundItem(int missingItemId, string? foundItemID = null)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateReportAssociatedFoundItemAsync(missingItemId, foundItemID, authenticatedUserUsername);

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

        /// <summary>
        ///   API endpoint to get counts of missing item reports.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="color"></param>
        /// <param name="category"></param>
        /// <param name="keywords"></param>
        /// <returns>Int - The number of missing items under the provided filters</returns>
        [HttpGet]
        [Route("missingitems/count")]
        public ActionResult<object> GetMissingItemsCount(string? status = null, string? color = null, string? category = null, string? keywords = null)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var count = lostAndFoundService.GetMissingItemsCount(authenticatedUserUsername, status, color, category, keywords);
            return Ok(count);
        }

        /// <summary>
        /// Get the list of found items assigned to the specified owner.
        /// If the "owner" query parameter is not provided, this returns items for the currently authenticated user.
        /// </summary>
        /// <param name="owner">
        /// Optional query parameter to filter by owner. If null or empty, uses the authenticated user's username.
        /// </param>
        /// <returns>
        /// An HTTP result containing an array of FoundItemViewModel objects if found; otherwise, NotFound.
        /// </returns>
        [HttpGet]
        [Route("founditems/owner")]
        public ActionResult<IEnumerable<FoundItemViewModel>> GetFoundItemsByOwner(string? owner = null)
        {
            // Retrieve the username of the currently authenticated user.
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            IEnumerable<FoundItemViewModel> result;

            // If no owner is specified, default to the authenticated user.
            if (string.IsNullOrWhiteSpace(owner))
            {
                result = lostAndFoundService.GetFoundItemsByOwner(authenticatedUserUsername, authenticatedUserUsername);
            }
            else
            {
                result = lostAndFoundService.GetFoundItemsByOwner(owner, authenticatedUserUsername);
            }

            // Return the result if found, otherwise return a NotFound response.
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
        /// Create a new found item.
        /// </summary>
        /// <param name="FoundItemDetails">The data of the found item to create</param>
        /// <returns>ObjectResult - the http status code result of the action, with the ID of the created found item</returns>
        [HttpPost]
        [Route(("founditems"))]
        public ActionResult<string> CreateFoundItemReport([FromBody] FoundItemViewModel FoundItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            string reportID = lostAndFoundService.CreateFoundItem(FoundItemDetails, authenticatedUserUsername);

            return Ok(reportID);
        }

        /// <summary>
        /// Create a new action for the found item with given ID.
        /// </summary>
        /// <param name="foundItemId">The id of the report to add an action to</param>
        /// <param name="FoundActionsTaken">The data for the new action</param>
        /// <returns>ObjectResult - the http status code result of the action, with the ID of the created action taken</returns>
        [HttpPost]
        [Route("founditems/{foundItemId}/actionstaken")]
        public ActionResult<int> CreateFoundActionTaken(string foundItemId, [FromBody] FoundActionsTakenViewModel FoundActionsTaken)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            int actionId = lostAndFoundService.CreateFoundActionTaken(foundItemId, FoundActionsTaken, authenticatedUserUsername);

            return Ok(actionId);
        }

        /// <summary>
        /// Update a found item with the given id with given data.
        /// </summary>
        /// <param name="itemId">The id of the item to update</param>
        /// <param name="FoundItemDetails">The found data to update the item with</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route(("founditems/{itemId}"))]
        public async Task<ActionResult> UpdateFoundItem(string itemId, [FromBody] FoundItemViewModel FoundItemDetails)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateFoundItemAsync(itemId, FoundItemDetails, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Update the status of the found item with given id to the given status text.
        /// </summary>
        /// <param name="itemId">The id of the item to update</param>
        /// <param name="status">The new status of the item</param>
        /// <returns>ObjectResult - the http status code result of the action</returns>
        [HttpPut]
        [Route("founditems/{itemId}/{status}")]
        public async Task<ActionResult> UpdateFoundStatus(string itemId, string status)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateFoundStatusAsync(itemId, status, authenticatedUserUsername);

            return Ok();
        }

        [HttpPut]
        [Route("founditems/{itemId}/linkReport/{missingReportID?}")]
        public async Task<ActionResult> UpdateFoundAssociatedMissingReport(string itemId, int? missingReportID = null)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            await lostAndFoundService.UpdateFoundAssociatedMissingReportAsync(itemId, missingReportID, authenticatedUserUsername);

            return Ok();
        }

        /// <summary>
        /// Get a single found item with given ID, including it's actions taken.
        /// </summary>
        /// <param name="itemID">The tag ID of the item to get</param>
        /// <returns>ObjectResult - the http status code result of the action, with the found item</returns>
        [HttpGet]
        [Route(("founditems/{itemID}"))]
        public ActionResult<FoundItemViewModel> GetFoundItem(string itemID)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            return Ok(lostAndFoundService.GetFoundItem(itemID, authenticatedUserUsername));
        }

        /// <summary>
        /// Get the list of found items, filtered by the provided filters.
        /// </summary>
        /// <param name="color">The selected color for filtering items</param>
        /// <param name="category">The selected category for filtering items</param>
        /// <param name="ID">The selected tag number/id for filtering by tag number</param>
        /// <param name="keywords">The selected keywords for filtering by items</param>
        /// <param name="status">The selected status for filtering items</param>
        /// <param name="latestDate">The latest date created that the list of reports should include</param>
        /// <returns>ObjectResult - an http status code, with an array of FoundItem objects in the body </returns>
        [HttpGet]
        [Route("founditems")]
        public ActionResult<IEnumerable<FoundItemViewModel>> GetFoundItems(DateTime? latestDate = null,
                                                                           string? status = null,
                                                                           string? color = null,
                                                                           string? category = null,
                                                                           string? ID = null,
                                                                           string? keywords = null)
        {
            IEnumerable<FoundItemViewModel> result;
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            // If no username specified in the query, get all items
            result = lostAndFoundService.GetFoundItemsAll(authenticatedUserUsername, latestDate, status, color, category, ID, keywords);

            return Ok(result);
        }

        /// <summary>
        ///   API endpoint to get counts of found items.
        /// </summary>
        /// <param name="latestDate"></param>
        /// <param name="status"></param>
        /// <param name="color"></param>
        /// <param name="category"></param>
        /// <param name="ID"></param>
        /// <param name="keywords"></param>
        /// <returns>Int - The number of found items under the provided filters</returns>
        [HttpGet]
        [Route("founditems/count")]
        public ActionResult<object> GetFoundItemsCount(DateTime? latestDate = null, string? status = null, string? color = null, string? category = null, string? ID = null, string? keywords = null)
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var count = lostAndFoundService.GetFoundItemsCount(authenticatedUserUsername, latestDate, status, color, category, ID, keywords);
            return Ok(count);
        }
    }
}
