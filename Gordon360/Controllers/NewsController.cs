using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : GordonControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>Gets a news item by id from the database</summary>
        /// <param name="newsID">The id of the news item to retrieve</param>
        /// <returns>The news item</returns>
        [HttpGet]
        [Route("{newsID}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.NEWS)]
        // Private route to authenticated users
        public ActionResult<StudentNewsViewModel> GetByID(int newsID)
        {
            // StateYourBusiness verifies that user is authenticated
            var result = (StudentNewsViewModel)_newsService.Get(newsID);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>Gets the base64 image data for an image corresponding 
        /// to a student news item. Only used by GO; when we move student news approval
        /// to 360, this will be removed.</summary>
        /// <param name="newsID">The id of the news item to retrieve image from</param>
        /// <returns>base64 string representing image</returns>
        [HttpGet]
        [Route("{newsID}/image")]
        public ActionResult<string> GetImage(int newsID)
        {
            var result = (StudentNewsViewModel)_newsService.Get(newsID);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.Image);
        }

        /** Call the service that gets all approved student news entries not yet expired, filtering
         * out the expired by comparing 2 weeks past date entered to current date
         */
        [HttpGet]
        [Route("not-expired")]
        public async Task<ActionResult<IOrderedEnumerable<StudentNewsViewModel>>> GetNotExpiredAsync()
        {
            var result = await _newsService.GetNewsNotExpiredAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets all new and approved student news entries
         * which have not already expired,
         * checking novelty by comparing an entry's date entered to 10am on the previous day
         */
        [HttpGet]
        [Route("new")]
        public async Task<ActionResult<IEnumerable<StudentNewsViewModel>>> GetNewAsync()
        {
            var result = await _newsService.GetNewsNewAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets the list of categories
         */
        [HttpGet]
        [Route("categories")]
        public ActionResult<IEnumerable<StudentNewsCategoryViewModel>> GetCategories()
        {
            var result = _newsService.GetNewsCategories();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("unapproved")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.NEWS)]
        public ActionResult<IEnumerable<StudentNewsViewModel>> GetNewsUnapproved()
        {
            var result = _newsService.GetNewsUnapproved();
            return Ok(result);
        }

        /** Call the service that gets all unapproved student news entries (by a particular user)
         * not yet expired, filtering out the expired news
         * @TODO: Remove redundant username/id from this and service
         * @TODO: fix documentation comments
         */
        [HttpGet]
        [Route("personal-unapproved")]
        public async Task<ActionResult<IEnumerable<StudentNewsViewModel>>> GetNewsPersonalUnapprovedAsync()
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            // Call appropriate service
            var result = await _newsService.GetNewsPersonalUnapprovedAsync(authenticatedUserUsername);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
      
        /** Create a new news item to be added to the database
         * @TODO: Remove redundant username/id from this and service
         * @TODO: fix documentation comments
         */
        [HttpPost]
        [Route("")]
        public ActionResult<StudentNews> Post([FromBody] StudentNewsUploadViewModel studentNewsUpload)
        {
            // Get authenticated username/id
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            var newsItem = new StudentNews
            {
                ADUN = authenticatedUserUsername,
                Subject = studentNewsUpload.Subject,
                categoryID = studentNewsUpload.categoryID,
                Body = studentNewsUpload.Body,
                Image = studentNewsUpload.Image,
            };

            // Call appropriate service
            var result = _newsService.SubmitNews(newsItem, authenticatedUserUsername);
            if (result == null)
            {
                return NotFound();
            }
            return Created("News", result);
        }

        /// <summary>Deletes a news item from the database</summary>
        /// <param name="newsID">The id of the news item to delete</param>
        /// <returns>The deleted news item</returns>
        /// <remarks>The news item must be authored by the user and must not be expired</remarks>
        [HttpDelete]
        [Route("{newsID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.NEWS)]
        // Private route to authenticated authors of the news entity
        public ActionResult<StudentNews> Delete(int newsID)
        {
            // StateYourBusiness verifies that user is authenticated
            // Delete permission should be allowed only to authors of the news item
            // News item must not have already expired
            var result = _newsService.DeleteNews(newsID);
            // Shouldn't be necessary
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// (Controller) Edits a news item in the database
        /// </summary>
        /// <param name="newsID">The id of the news item to edit</param>
        /// <param name="studentNewsEdit">The news object that contains updated values</param>
        /// <returns>The updated news item</returns>
        /// <remarks>The news item must be authored by the user and must not be expired and must be unapproved</remarks>
        [HttpPut]
        [Route("{newsID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.NEWS)]
        // Private route to authenticated users - authors of posting or admins
        public ActionResult<StudentNewsViewModel> EditPosting(int newsID, [FromBody] StudentNewsUploadViewModel studentNewsEdit)
        {
            // StateYourBusiness verifies that user is authenticated
            var result = _newsService.EditPosting(newsID, studentNewsEdit);
            return Ok(result);
        }

        [HttpPut]
        [Route("{newsID}/image")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.NEWS)]
        // Private route to authenticated users - authors of posting or admins
        public ActionResult<StudentNewsViewModel> EditPostingImage(int newsID, [FromBody] StudentNewsImageUploadViewModel studentNewsImageEdit)
        {
            // StateYourBusiness verifies that user is authenticated
            var result = _newsService.EditImage(newsID, studentNewsImageEdit);
            return Ok(result);
        }

        /// <summary>
        ///  Approve or deny a news posting in the database
        /// </summary>
        /// <param name="newsID">The id of the news item to approve</param>
        /// <param name="newsStatusAccepted">The accept status that will apply to the news item</param>
        /// <returns>The approved or denied news item</returns>
        /// <remarks>The news item must not be expired</remarks>
        [HttpPut]
        [Route("{newsID}/accepted")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.NEWS_APPROVAL)]
        public ActionResult<StudentNewsViewModel> UpdateAcceptedStatus(int newsID, [FromBody] bool newsStatusAccepted)
        {
            var result = _newsService.AlterPostAcceptStatus(newsID, newsStatusAccepted);
            return Ok(result);
        }
    }
}
