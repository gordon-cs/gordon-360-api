using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using System.Diagnostics;

//TODO change all "Apartment" occurences to "StudentHousing" 
// or maybe just "Housing" since any Housing data related to Gordon should be for students anyway (RDs and ACs, though?)
//Create StudentHousingInfoRepository in UnitOfWork

namespace Gordon360.Services
{
    public class ApartmentInfoService : IApartmentInfoService
    {
        // TODO talk styling and removing random spaces

        private IUnitOfWork _unitOfWork;

        public ApartmentInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets student housing info
        /// TODO list what exactly we mean by houding info
        /// </summary>
        /// TODO remove next line and parameter newsID
        /// <param name="ID">The ID (id of student)</param>
        /// <returns>The news item</returns>
        public IEnumerable Get(int newsID) //TODO IEnumerable needs <StudentHousingInfoViewModel>
        {
            var studentHousingItem = _unitOfWork.ApartmentInfoRepository.Get();
            //TODO just return what's to the right of the '=' since it was only two lines in order to check for null
            // scratch THAT, I led you in the wrong direction: we didn't call the stored procedure we made!!
            // we should call this getAll and make it like News_Not_Expired
            return studentHousingItem;
        }
    }
}
