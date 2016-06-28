using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class AccountService : IAccountService
    {
        private IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ACCOUNT Get(string id)
        {
            var result = _unitOfWork.AccountRepository.GetById(id);
            return result;
        }

        public IEnumerable<ACCOUNT> GetAll()
        {
            var result = _unitOfWork.AccountRepository.GetAll();
            return result;
        }
    }
}