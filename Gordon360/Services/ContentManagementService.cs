using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using Gordon360.Database.CCT;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Utils;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class that facilitates data (specifically, site content) passing between the ContentManagementController and the database model.
    /// </summary>
    public class ContentManagementService : IContentManagementService
    {
        private CCTContext _context;
        private readonly ImageUtils _imageUtils = new ImageUtils();

        private readonly string SlideUploadPath = "browseable/slider";


        public ContentManagementService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public IEnumerable<SliderViewModel> DEPRECATED_GetSliderContent()
        {
           return _context.Slider_Images.Select<Slider_Images, SliderViewModel>(x => x);
        }

        /// <summary>
        /// Retrieve all banner slides from the database
        /// </summary>
        /// <returns>An IEnumerable of the slides in the database</returns>
        public IEnumerable<Slider_Images> GetBannerSlides() => _unitOfWork.SliderRepository.GetAll();

        /// <summary>
        /// Inserts a banner slide in the database and uploads the image to the local slider folder
        /// </summary>
        /// <param name="slide">The slide to add</param>
        /// <param name="serverURL">The url of the server that the image is being posted to.
        /// This is needed to save the image path into the database. The URL is different depending on where the API is running.
        /// The URL is trivial to access from the controller, but not available from within the service, so it has to be passed in.
        /// </param>
        /// <returns>The inserted slide</returns>
        public Slider_Images AddBannerSlide(BannerSlidePostViewModel slide, string serverURL)
        {
            Match match = Regex.Match(slide.ImageData, @"^data:image/(?<filetype>jpeg|jpg|png);base64,");
            if (!match.Success)
            {
                throw new BadInputException();
            }
            string filetype = match.Groups["filetype"].Value;
            var imageFormat = filetype == "png" ? ImageFormat.Png : ImageFormat.Jpeg;
            string fileName = $"{slide.Title}.{filetype}";

            string localImagePath = $"{HttpContext.Current.Server.MapPath($"~/{SlideUploadPath}")}/{fileName}";
            string rawImageData = slide.ImageData.Substring(match.Length);
            _imageUtils.UploadImage(localImagePath, rawImageData, imageFormat);

            var entry = new Slider_Images
            {
                Path = $"{serverURL}{SlideUploadPath}/{fileName}",
                Title = slide.Title,
                LinkURL = slide.LinkURL,
                SortOrder = slide.SortOrder,
                Width = 1500,
                Height = 600
            };
            _unitOfWork.SliderRepository.Add(entry);
            _unitOfWork.Save();
            return entry;
        }

        /// <summary>
        /// Deletes a banner slide from the database and deletes the local image file
        /// </summary>
        /// <returns>The deleted slide</returns>
        public Slider_Images DeleteBannerSlide(int slideID)
        {
            var slide = _unitOfWork.SliderRepository.GetById(slideID);
            _imageUtils.DeleteImage(slide.Path);
            _unitOfWork.SliderRepository.Delete(slide);
            _unitOfWork.Save();
            return slide;
        }
    }
}