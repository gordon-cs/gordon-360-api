using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class that facilitates data (specifically, site content) passing between the ContentManagementController and the database model.
    /// </summary>
    public class ContentManagementService : IContentManagementService
    {
        private IUnitOfWork _unitOfWork;

        public ContentManagementService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches the dashboard slider content from the database.
        /// </summary>
        /// <returns>If found, returns a set of SliderViewModel's, based on each slide entry in the db. 
        /// If not returns an empty IEnumerable.</returns>
        public IEnumerable<SliderViewModel> GetSliderContent()
        {
           var query =  _unitOfWork.SliderRepository.GetAll();
           var result = query.Select<C360_SLIDER, SliderViewModel>(x => x);

           return result;
        }
    }
}