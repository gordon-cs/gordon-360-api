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

        public IEnumerable<StudentNewsViewModel> GetNewsNotExpired();

        public IEnumerable<StudentNewsViewModel> GetNewsNew();

        public IEnumerable<StudentNewsCategory> GetNewsCategories();
    }
}