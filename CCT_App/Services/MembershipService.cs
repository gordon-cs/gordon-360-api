using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;
namespace CCT_App.Services
{
    public class MembershipService : IMembershipService
    {
        private IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Membership Add(Membership membership)
        {
            throw new NotImplementedException();
        }

        public Membership Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Membership Get(int id)
        {
            var result = _unitOfWork.MembershipRepository.GetById(id);
            return result;
        }

        public IEnumerable<Membership> GetAll()
        {
            var result = _unitOfWork.MembershipRepository.GetAll();
            return result;
        }

        public Membership Update(int id, Membership membership)
        {
            throw new NotImplementedException();
        }
    }
}