using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private IUnitOfWork _unitOfWork;

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public IEnumerable<NewsViewModel> GetNewsNotExpired();
        //{}


        //public IEnumerable<NewsViewModel> GetNewsForDay(string date);
        //{}
    }
}