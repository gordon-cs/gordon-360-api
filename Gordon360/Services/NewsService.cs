using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private IUnitOfWork _unitOfWork;

        private CCTEntities1 _context;

        private static readonly string UploadsFolder = "/browseable/uploads/news/";
        private readonly string FolderPath = HttpContext.Current.Server.MapPath("~" + UploadsFolder);

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _context = new CCTEntities1();
        }

        /// <summary>
        /// Gets a news item entity by id
        /// NOTE: Also a helper method, hence why it returns a StudentNews model
        /// rather than a StudentNewsViewModel - must be casted as the latter in its own
        /// controller
        /// </summary>
        /// <param name="newsID">The SNID (id of news item)</param>
        public StudentNews Get(int newsID)
        {
            var newsItem = _unitOfWork.StudentNewsRepository.GetById(newsID);
            // Thrown exceptions will be converted to HTTP Responses by the CustomExceptionFilter
            if (newsItem == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item was not found." };
            }

            string imagePath = newsItem.Image;

            return newsItem;
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNotExpired()
        {
            IEnumerable<StudentNewsViewModel> items = RawSqlQuery<StudentNewsViewModel>.query("NEWS_NOT_EXPIRED");
            return InsertImageData(items);
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNew()
        {
            IEnumerable<StudentNewsViewModel> items = RawSqlQuery<StudentNewsViewModel>.query("NEWS_NEW");
            return InsertImageData(items);
        }

        public IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories()
        {
            return RawSqlQuery<StudentNewsCategoryViewModel>.query("NEWS_CATEGORIES");
        }

        /// <summary>
        /// Gets unapproved unexpired news submitted by user.
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="username">username</param>
        /// <returns>Result of query</returns>
        public IEnumerable<StudentNewsViewModel> GetNewsPersonalUnapproved(string id, string username)
        {
            // Verify account
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            // Query the database
            var usernameParam = new SqlParameter("@Username", username);

            IEnumerable<StudentNewsViewModel> items = RawSqlQuery<StudentNewsViewModel>.query("NEWS_PERSONAL_UNAPPROVED @Username", usernameParam);
            
            return InsertImageData(items);
        }


        /// <summary>
        /// Adds a news item record to storage.
        /// </summary>
        /// <param name="newsItem">The news item to be added</param>
        /// <param name="username">username</param>
        /// <param name="id">id</param>
        /// <returns>The newly added Membership object</returns>
        public StudentNews SubmitNews(StudentNews newsItem, string username, string id)
        {
            // Not currently used
            ValidateNewsItem(newsItem);

            VerifyAccount(id);

            // SQL Parameters
            var usernameParam = new SqlParameter("@Username", username);
            var categoryIDParam = new SqlParameter("@CategoryID", newsItem.categoryID);
            var subjectParam = new SqlParameter("@Subject", newsItem.Subject);
            var bodyParam = new SqlParameter("@Body", newsItem.Body);
            var imageParam = new SqlParameter("@Image", "");

            // Run stored procedure
            IEnumerable<int> idResult = _context.Database.SqlQuery<int>("INSERT_NEWS_ITEM @Username, @CategoryID, @Subject, @Body, @Image", usernameParam, categoryIDParam, subjectParam, bodyParam, imageParam);

            int snid = idResult.FirstOrDefault();

            UploadImage(newsItem.Image, snid);

            return newsItem;
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

            RemoveImage(newsItem.SNID);

            var result = _unitOfWork.StudentNewsRepository.Delete(newsItem);
            _unitOfWork.Save();
            return result;
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
            if(newData.Subject == null || newData.Body == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The new data to update the news item is missing some entries." };
            }

            newsItem.categoryID = newData.categoryID;
            newsItem.Subject = newData.Subject;
            newsItem.Body = newData.Body;

            if (newData.Image != null)
            {
                UploadImage(newData.Image, newsID);
            }

            //If the image property is null, it means that either the user
            //chose to remove the previous image or that there was no previous
            //image (RemoveImage is designed to handle this).
            else
            {
                RemoveImage(newsItem.SNID);
                newsItem.Image = newData.Image;
            }

            _unitOfWork.Save();
            
            return newsItem;
        }

        /// <summary>
        /// Takes a filepath for an image, navigates to it, collects the raw data
        /// of the file and converts it to base64 format. 
        /// 
        /// The base64 data will not include the first part of a base64 image ("data:image/...").
        /// This is because this part is removed in every image before being submitted, and it
        /// is readded in the frontend before being displayed.
        /// 
        /// This helper function does not perform any error checking; every place that calls it
        /// checks that the path is not empty. Theoretically if it isn't empty it's certainly a valid path.
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <returns>The base64 content of the image</returns>
        public string GetBase64ImageDataFromPath(string imagePath)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
            using (MemoryStream data = new MemoryStream())
            {
                image.Save(data, image.RawFormat);
                byte[] imageBytes = data.ToArray();
                string base64Data = System.Convert.ToBase64String(imageBytes);
                return base64Data;
            }
        }

        /// <summary>
        /// Iterates through a collection of news items that have just been queried from
        /// the database, replacing the Image entry of each (which contains the path to an image)
        /// with the image itself. This is done because when the frontend makes requests for news items,
        /// it needs the image data and not the path, but we store the path in the DB for efficiency and
        /// storage reasons.
        /// </summary>
        /// <param name="items">The collection of student news entries</param>
        /// <returns>The collection of entries with base64 data in their image variable</returns>
        private IEnumerable<StudentNewsViewModel> InsertImageData(IEnumerable<StudentNewsViewModel> items)
        {
            foreach (StudentNewsViewModel item in items)
            {
                string imagePath = item.Image;

                if (imagePath != null)
                {
                    item.Image = GetBase64ImageDataFromPath(imagePath);
                }   
            }
            return items;
        }

        /// <summary>
        /// Uploads a news image
        /// Can be used to add an image to a new posting or to replace an image
        /// for an existing posting.
        /// </summary>
        /// <param name="imageData">The base64 image data to be stored</param>
        /// <param name="snid">The SNID of the news item to which the image belongs</param>
        private void UploadImage(string imageData, int snid)
        {
            if (imageData == null) { return; }

            var newsPost = Get(snid);

            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath);
            }

            string fileName = snid + ".jpg";
            string imagePath = FolderPath + fileName;

            byte[] imageDataArray = System.Convert.FromBase64String(imageData);

            try
            {
                //First, if the Image path is empty- a new post- and the imageData is empty,
                //It means a news submission was made without a picture, so we won't uploade anything.
                // We'll just return.
                if (imageData == null && newsPost.Image == null)
                {
                    return;
                }

                System.Diagnostics.Debug.WriteLine(newsPost.Image + " is value ");
                //If a new post, Image path will be null. If there is image data,
                //we need to conitune, so first we add the path to the DB column. 
                if (newsPost.Image == null && imageData != null)
                {
                    newsPost.Image = imagePath;
                }

                _unitOfWork.Save();

                //Load the image data into a memory stream and save it to
                //the appropriate file:
                using (MemoryStream imageStream = new MemoryStream(imageDataArray))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);

                    System.Diagnostics.Debug.WriteLine(imagePath);
                    System.Diagnostics.Debug.WriteLine(image);

                    image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return;//Saving image was successful
                }
            }

            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("Something went wrong trying to save the image for submission with SNID " + snid);
            }
        }

        /// <summary>
        /// Deletes an image from the filesystem, if there is one.
        /// </summary>
        /// <param name="snid">The SNID for the news entry to which the image belonged.</param>
        private void RemoveImage(int snid)
        {
            string fileName = snid + ".jpg";
            string imagePath = FolderPath + fileName;
            try
            {
                File.Delete(imagePath);
            }
            catch (System.Exception e)
            {
                //If there wasn't an image there, the only reason
                //was that no image was associated with the news item,
                //so this catch handles that and there's no cause for concern.
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Helper method to verify that a given news item has not yet been approved
        /// </summary>
        /// <param name="newsItem">The news item to verify</param>
        /// <returns>true if unapproved, otherwise throws some kind of meaningful exception</returns>
        private bool VerifyUnapproved(StudentNews newsItem)
        {
            // Note: This check has been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            if (newsItem.Accepted == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item acceptance status could not be verified." };
            }
            if(newsItem.Accepted == true)
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
        private bool VerifyUnexpired(StudentNews newsItem)
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
                throw new Exceptions.CustomExceptions.UnauthorizedAccessException() { ExceptionMessage = "Unauthorized to delete expired news items." };
            }
            return true;
        }

        /// <summary>
        /// Helper method to validate a news item
        /// </summary>
        /// <param name="newsItem">The news item to validate</param>
        /// <returns>True if valid. Throws ResourceNotFoundException if not. Exception is caught in an Exception Filter</returns>
        private bool ValidateNewsItem(StudentNews newsItem)
        {
            // any input sanitization should go here

            return true;
        }

        /// <summary>
        /// Verifies that a student account exists
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>true if account exists, ResourceNotFoundException if null</returns>
        private bool VerifyAccount(string id)
        {
            // Verify account
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            return true;
        }
    }
}
