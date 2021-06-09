using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Models.ViewModels;

using System;
using System.Net.Http;
using System.Web;
using System.Net;
using Gordon360.Providers;
using System.IO;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/news")]
    [Authorize]
    [CustomExceptionFilter]
    public class NewsController : ApiController
    {
        private INewsService _newsService;
        private IAccountService _accountService;

        /**private void catchBadInput()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(variable))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
        }*/

        // Constructor
        public NewsController()
        {
            // Connect to service through which data (from the database) can be accessed 
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _newsService = new NewsService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
        }

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
        public IHttpActionResult GetByID(int newsID)
        {
            // StateYourBusiness verifies that user is authenticated
            var result = (StudentNewsViewModel)_newsService.Get(newsID);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets all approved student news entries not yet expired, filtering
         * out the expired by comparing 2 weeks past date entered to current date
         */
        [HttpGet]
        [Route("not-expired")]
        public IHttpActionResult GetNotExpired()
        {
            var result = _newsService.GetNewsNotExpired();
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
        public IHttpActionResult GetNew()
        {
            var result = _newsService.GetNewsNew();
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
        public IHttpActionResult GetCategories()
        {
            var result = _newsService.GetNewsCategories();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Call the service that gets all unapproved student news entries (by a particular user)
         * not yet expired, filtering out the expired news
         */
        [HttpGet]
        [Route("personal-unapproved")]
        public IHttpActionResult GetNewsPersonalUnapproved()
        {
            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            
            // Call appropriate service
            var result = _newsService.GetNewsPersonalUnapproved(id, username);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /** Create a new news item to be added to the database
         */
        [HttpPost]
        [Route("")]//Might need to add here - Josh
        public IHttpActionResult Post([FromBody] StudentNews newsItem)
        {
            // Check for bad input
            if (!ModelState.IsValid || newsItem == null )
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }
                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            // Get authenticated username/id
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = _accountService.GetAccountByUsername(username).GordonID;
            //!!!!

            //Move Image to storage
            var uploadsFolder = "/browseable/uploads/new" + id + "/";
            
            // if (!Request.Content.IsMimeMultipartContent())
            // {
            //     throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            // }
            

            var existingFile = "";
            
            if(!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~" + uploadsFolder)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
            }
            else
            {
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
                    
                    foreach (FileInfo file in di.GetFiles())
                    {
                        existingFile = file.Name;
                        file.Delete();
                    }
                }
                catch (System.Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            string root = HttpContext.Current.Server.MapPath("~" + uploadsFolder);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                // await Request.Content.ReadAsMultipartAsync(provider);
                Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", "");

                    // If the file has the same name as the previous one, add a 1 at the end to make it different (so that the browser does not cache it)
                    if (fileName.Equals(existingFile))
                    {
                        var oldFileName = fileName;

                        // Add "1" before the extension
                        fileName = fileName.Substring(0, fileName.LastIndexOf('.')) + "1" + fileName.Substring(fileName.LastIndexOf('.'));

                        // Rename existing File
                        System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
                        System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName);
                    }

                    var uploadPath = uploadsFolder + fileName;
                    var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                    var imagePath = baseUrl + uploadPath;
                    //_activityService.UpdateActivityImage(id, imagePath);
                    newsItem.Image = imagePath;
                }
                //return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");
            }

            // Call appropriate service
            var result = _newsService.SubmitNews(newsItem, username, id);
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
        public IHttpActionResult Delete(int newsID)
        {
            // StateYourBusiness verifies that user is authenticated
            // Delete permission should be allowed only to authors of the news item
            // News item must not have already expired
            var result = _newsService.DeleteNews(newsID);
            // Shouldn't be necessary
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// (Controller) Edits a news item in the database
        /// </summary>
        /// <param name="newsID">The id of the news item to edit</param>
        /// <param name="newData">The news object that contains updated values</param>
        /// <returns>The updated news item</returns>
        /// <remarks>The news item must be authored by the user and must not be expired and must be unapproved</remarks>
        [HttpPut]
        [Route("{newsID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.NEWS)]
        // Private route to authenticated users - authors of posting or admins
        public IHttpActionResult EditPosting(int newsID,[FromBody] StudentNews newData)
        {
            // StateYourBusiness verifies that user is authenticated
            var result = _newsService.EditPosting(newsID, newData);
            return Ok(result);
        }

    }
}
