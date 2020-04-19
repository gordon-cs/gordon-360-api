using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gordon360.Controllers.Api
{
    public class NewsController : ApiController
    {
        private INewsService _newsService;

        public NewsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _newsService = new NewsService(_unitOfWork);
        }

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
    }
}