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
        private IUnitOfWork _unitOfWork;

        public ApartmentInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets student housing info
        /// </summary>
        /// <returns>TODO list what exactly we mean by houding info</returns>
        public IEnumerable<StudentHousingInfoViewModel> GetAll()
        {
            var studentHousingItem = _unitOfWork.ApartmentInfoRepository.Get();
            // TODO  I led you in the wrong direction: we didn't call the stored procedure we made!!
            // we should make it like News_Not_Expired
            return studentHousingItem;
        }
    }
}
