using Gordon360.Models.CCT.Context;
using Gordon360.Models.MyGordon.Context;
using Gordon360.Exceptions;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private readonly MyGordonContext _context;
        private readonly CCTContext _contextCCT;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ServerUtils _serverUtils;

        private string NewsUploadsPath => Path.Combine(_webHostEnvironment.ContentRootPath, "browseable/uploads/news");

        public NewsService(MyGordonContext context, CCTContext contextCCT, IWebHostEnvironment webHostEnvironment, ServerUtils serverUtils)
        {
            _context = context;
            _contextCCT = contextCCT;
            _webHostEnvironment = webHostEnvironment;
            _serverUtils = serverUtils;
        }

        /// <summary>
        /// Gets a news item entity by id
        /// NOTE: Also a helper method, hence why it returns a StudentNews model
        /// rather than a StudentNewsViewModel - must be casted as the latter in its own
        /// controller
        /// </summary>
        /// <param name="newsID">The SNID (id of news item)</param>
        /// <returns>The news item</returns>
        public StudentNews Get(int newsID)
        {
            var newsItem = _context.StudentNews.Find(newsID);
            // Thrown exceptions will be converted to HTTP Responses by the CustomExceptionFilter
            if (newsItem == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item was not found." };
            }

            return newsItem;
        }

        public async Task<IEnumerable<StudentNewsViewModel>> GetNewsNotExpiredAsync()
        {
            var news = await _contextCCT.Procedures.NEWS_NOT_EXPIREDAsync();
            return news.Select(n => new StudentNewsViewModel
            {
                SNID = n.SNID,
                ADUN = n.ADUN,
                categoryID = n.categoryID,
                Subject = n.Subject,
                Body = n.Body,
                Image = n.Image,
                Accepted = true,
                Sent = n.Sent,
                thisPastMailing = n.thisPastMailing,
                Entered = n.Entered,
                categoryName = n.categoryName,
                SortOrder = n.SortOrder,
                ManualExpirationDate = n.ManualExpirationDate,
            });
        }

        public async Task<IEnumerable<StudentNewsViewModel>> GetNewsNewAsync()
        {
            var news = await _contextCCT.Procedures.NEWS_NEWAsync();
            return news.Select(n => new StudentNewsViewModel
            {
                SNID = n.SNID,
                ADUN = n.ADUN,
                categoryID = n.categoryID,
                Subject = n.Subject,
                Body = n.Body,
                Image = n.Image,
                Accepted = true,
                Sent = n.Sent,
                thisPastMailing = n.thisPastMailing,
                Entered = n.Entered,
                categoryName = n.categoryName,
                SortOrder = n.SortOrder,
                ManualExpirationDate = n.ManualExpirationDate,
            });
        }

        public IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories()
        {
            return _context.StudentNewsCategory.OrderBy(c => c.SortOrder).Select<StudentNewsCategory, StudentNewsCategoryViewModel>(c => c);
        }

        /// <summary>
        /// Gets unapproved unexpired news submitted by user.
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>Result of query</returns>
        public async Task<IEnumerable<StudentNewsViewModel>> GetNewsPersonalUnapprovedAsync(string username)
        {
            // Verify account
            var account = await _contextCCT.ACCOUNT.FirstOrDefaultAsync(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var news = await _contextCCT.Procedures.NEWS_PERSONAL_UNAPPROVEDAsync(username);
            return news.Select(n => new StudentNewsViewModel
            {
                SNID = n.SNID,
                ADUN = n.ADUN,
                categoryID = n.categoryID,
                Subject = n.Subject,
                Body = n.Body,
                Image = n.Image,
                Accepted = true,
                Sent = n.Sent,
                thisPastMailing = n.thisPastMailing,
                Entered = n.Entered,
                categoryName = n.categoryName,
                SortOrder = n.SortOrder,
                ManualExpirationDate = n.ManualExpirationDate,
            });
        }

        /// <summary>
        /// Adds a news item record to storage.
        /// </summary>
        /// <param name="newsItem">The news item to be added</param>
        /// <param name="username">username</param>
        /// <returns>The newly added Membership object</returns>
        public StudentNews SubmitNews(StudentNews newsItem, string username)
        {
            // Not currently used
            ValidateNewsItem(newsItem);

            VerifyAccount(username);

            var itemToSubmit = new StudentNews
            {
                categoryID = newsItem.categoryID,
                Subject = newsItem.Subject,
                Body = newsItem.Body,
                Image = newsItem.Image,
                ADUN = username,
                Accepted = false,
                Sent = true,
                thisPastMailing = false,
                Entered = DateTime.Now
            };

            _context.StudentNews.Add(itemToSubmit);
            if (itemToSubmit.Image != null)
            {
                // Put current DateTime in filename so the browser knows it's a new file and refreshes cache
                var filename = Guid.NewGuid().ToString("N") + ".png";
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "browseable", "uploads", filename);

                var serverAddress = _serverUtils.GetAddress();
                if (serverAddress is not string) throw new Exception("Could not upload Student News Image: Server Address is null");

                var url = $"{serverAddress}/browseable/uploads/{filename}";

                //delete old image file if it exists.
                if (Path.GetDirectoryName(imagePath) is string directory && Directory.Exists(directory))
                {
                    foreach (FileInfo file in new DirectoryInfo(directory).GetFiles())
                    {
                        file.Delete();
                    }
                }

                ImageUtils.UploadImage(imagePath, itemToSubmit.Image);

                itemToSubmit.Image = url;
            }

            _context.SaveChanges();

            return itemToSubmit;
        }

        /// <summary>
        /// (Service) Deletes a news item from the database
        /// </summary>
        /// <param name="newsID">The id of the news item to delete</param>
        /// <returns>The deleted news item</returns>
        /// <remarks>The news item must be authored by the user and must not be expired</remarks>
        public StudentNews DeleteNews(int newsID)
        {
            // Service method 'Get' throws its own exceptions
            var newsItem = Get(newsID);

            // Note: This check has been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            VerifyUnexpired(newsItem);

            if (newsItem.Image != null)
            {
                string fileName = newsItem.SNID + ".jpg";
                string imagePath = NewsUploadsPath + fileName;
                ImageUtils.DeleteImage(imagePath);
            }
            _context.StudentNews.Remove(newsItem);
            _context.SaveChanges();
            return newsItem;
        }

        /// <summary>
        /// (Service) Edits a news item in the database
        /// </summary>
        /// <param name="newsID">The id of the news item to edit</param>
        /// <param name="newData">The news object that contains updated values</param>
        /// <returns>The updated news item's view model</returns>
        /// <remarks>The news item must be authored by the user and must not be expired and must be unapproved</remarks>
        public StudentNewsViewModel EditPosting(int newsID, StudentNews newData)
        {
            // Service method 'Get' throws its own exceptions
            var newsItem = Get(newsID);

            // Note: These checks have been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            VerifyUnexpired(newsItem);
            VerifyUnapproved(newsItem);

            // categoryID is required, not nullable in StudentNews model
            if (newData.Subject == null || newData.Body == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The new data to update the news item is missing some entries." };
            }

            newsItem.categoryID = newData.categoryID;
            newsItem.Subject = newData.Subject;
            newsItem.Body = newData.Body;

            if (newData.Image != null)
            {
                string fileName = newsItem.SNID + ".jpg";
                string imagePath = NewsUploadsPath + fileName;
                ImageUtils.UploadImage(imagePath, newData.Image);
                newsItem.Image = imagePath;
            }

            //If the image property is null, it means that either the user
            //chose to remove the previous image or that there was no previous
            //image (DeleteImage is designed to handle this).
            else
            {
                string fileName = newsItem.SNID + ".jpg";
                string imagePath = NewsUploadsPath + fileName;
                ImageUtils.DeleteImage(imagePath);
                newsItem.Image = newData.Image;//null
            }
            _context.SaveChanges();

            return newsItem;
        }

        /// <summary>
        /// Helper method to verify that a given news item has not yet been approved
        /// </summary>
        /// <param name="newsItem">The news item to verify</param>
        /// <returns>true if unapproved, otherwise throws some kind of meaningful exception</returns>
        private static bool VerifyUnapproved(StudentNews newsItem)
        {
            // Note: This check has been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            if (newsItem.Accepted == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item acceptance status could not be verified." };
            }
            if (newsItem.Accepted == true)
            {
                throw new BadInputException() { ExceptionMessage = "The news item has already been approved." };
            }
            return true;
        }

        /// <summary>
        /// Helper method to verify that a given news item has not expired 
        /// (see documentation for expiration definition)
        /// </summary>
        /// <param name="newsItem">The news item to verify</param>
        /// <returns>true if unexpired, otherwise throws some kind of meaningful exception</returns>
        private static bool VerifyUnexpired(StudentNews newsItem)
        {
            // DateTime of date entered is nullable, so we need to check that here before comparing
            // If the entered date is null we shouldn't allow deletion to be safe
            // Note: This check has been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            if (newsItem.Entered == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item date could not be verified." };
            }
            var todaysDate = DateTime.Now;
            var newsDate = (DateTime)newsItem.Entered;
            var dateDiff = (todaysDate - newsDate).Days;
            if (dateDiff >= 14)
            {
                throw new UnauthorizedAccessException("Unauthorized to delete expired news items.");
            }
            return true;
        }

        /// <summary>
        /// Helper method to validate a news item
        /// </summary>
        /// <param name="newsItem">The news item to validate</param>
        /// <returns>True if valid. Throws ResourceNotFoundException if not. Exception is caught in an Exception Filter</returns>
        private static bool ValidateNewsItem(StudentNews newsItem)
        {
            // any input sanitization should go here

            return true;
        }

        /// <summary>
        /// Verifies that a student account exists
        /// </summary>
        /// <param name="username">The AD Username of the student</param>
        /// <returns>true if account exists, ResourceNotFoundException if null</returns>
        private bool VerifyAccount(string username)
        {
            // Verify account
            var account = _contextCCT.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            return true;
        }
    }
}
