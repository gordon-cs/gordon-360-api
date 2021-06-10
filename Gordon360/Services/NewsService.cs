using System.Net.Http;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using System.Web;
using System.Net;
using Gordon360.Providers;
using System.IO;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private IUnitOfWork _unitOfWork;

        private CCTEntities1 _context;

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
        /// <returns>The news item</returns>
        public StudentNews Get(int newsID)
        {
            var newsItem = _unitOfWork.StudentNewsRepository.GetById(newsID);
            // Thrown exceptions will be converted to HTTP Responses by the CustomExceptionFilter
            if (newsItem == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item was not found." };
            }
            return newsItem;

            //HMM
            //Get needs to return the image path, but the frontend needs the data. oh boy
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNotExpired()
        {
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_NOT_EXPIRED");
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNew()
        {
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_NEW");
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
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_PERSONAL_UNAPPROVED @Username", usernameParam);
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
            var imageParam = new SqlParameter("@Image", null);
            //TODO: Figure out this stupid image null thing.

            // Run stored procedure
            //var result = RawSqlQuery<StudentNewsViewModel>.query("INSERT_NEWS_ITEM @Username, @CategoryID, @Subject, @Body, @Image", usernameParam, categoryIDParam, subjectParam, bodyParam, imageParam);
            IEnumerable<int> idResult = _context.Database.SqlQuery<int>("INSERT_NEWS_ITEM @Username, @CategoryID, @Subject, @Body, @Image", usernameParam, categoryIDParam, subjectParam, bodyParam, imageParam);
            // if (idResult == null)
            // {
            //     throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            // }
            //Could maybe add tests one day
            int snid = idResult.FirstOrDefault();

            uploadImage(newsItem.Image, snid);

            return newsItem;
        }

        /// <summary>
        /// Uploads a news image
        /// Can be used to add an image to a new posting or to replace an image
        /// for an existing posting.
        /// </summary>
        /// <param name="imageData">The base64 image data to be stored</param>
        /// <param name="snid">The SNID of the news item to which the image belongs</param>
        /// <returns>The status successful or failure</returns>
        private string uploadImage(string imageData, int snid)
        {
            var newsPost = Get(snid);
            var uploadsFolder = "/browseable/uploads/news/";

            string folderPath = HttpContext.Current.Server.MapPath("~" + uploadsFolder);
            if(!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

            string fileName = snid + ".jpg";
            string imagePath = folderPath + fileName;

            byte[] imageDataArray = System.Convert.FromBase64String(imageData);

            try{
                //First, if the Image path is empty- a new post- and the imageData is empty,
                //It means a news submission was made without a picture, so we won't uploade anything.
                // We'll just return.
                if (imageData == null && newsPost.Image == null){
                    return "This submission had no image.";
                }

                //If a new post, Image path will be null. If there is image data,
                //we need to conitune, so first we add the path to the DB column. 
                if (newsPost.Image == null && imageData != null){
                    newsPost.Image = imagePath;
                }

                _unitOfWork.Save();

                //Load the image data into a memory stream and save it to
                //the appropriate file:
                MemoryStream imageStream = new MemoryStream(imageDataArray);
                System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);

                image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                return "Saving the image was successful.";
            }

            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return "Something went wrong trying to save the image for  submission with SNID " + snid;
            }
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

            //Delete image from filesystem, if there is one
            var uploadsFolder = "/browseable/uploads/news/";
            string folderPath = HttpContext.Current.Server.MapPath("~" + uploadsFolder);
            string fileName = newsItem.SNID + ".jpg";
            string imagePath = folderPath + fileName;
            try{
                File.Delete(imagePath);
            }
            catch(System.Exception e){
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

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
            uploadImage(newData.Image, newsID);

            _unitOfWork.Save();
            
            return (StudentNewsViewModel)newsItem;
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
