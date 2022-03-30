using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
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
        private IUnitOfWork _unitOfWork;
        private readonly ImageUtils _imageUtils = new ImageUtils();

        private readonly string SlideUploadPath = HttpContext.Current.Server.MapPath("~/browseable/slider/");

        public ContentManagementService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public IEnumerable<SliderViewModel> DEPRECATED_GetSliderContent()
        {
            var query = _unitOfWork.SliderRepository.GetAll();
            var result = query.Select<Slider_Images, SliderViewModel>(x => x);

            return result;
        }

        /// <summary>
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public IEnumerable<Slider_Images> GetBannerSlides() => _unitOfWork.SliderRepository.GetAll();

        /// <summary>
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public Slider_Images AddBannerSlide(BannerSlidePostViewModel slide)
        {
            string fileName = slide.Title + ".jpg";
            string imagePath = SlideUploadPath + fileName;
            _imageUtils.UploadImage(imagePath, slide.ImageData);

            var entry = new Slider_Images
            {
                Path = imagePath,
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
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public Slider_Images DeleteBannerSlide(int slideID)
        {
            var slide = _unitOfWork.SliderRepository.GetById(slideID);
            _unitOfWork.SliderRepository.Delete(slide);
            _unitOfWork.Save();
            return slide;
        }
    }
}