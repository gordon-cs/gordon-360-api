using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;
namespace CCT_App.Services
{
    public class SupervisorService : ISupervisorService
    {
        private IUnitOfWork _unitOfWork;

        public SupervisorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SUPERVISOR Add(SUPERVISOR supervisor)
        {
            throw new NotImplementedException();
        }

        public SUPERVISOR Delete(int id)
        {
            throw new NotImplementedException();
        }

        public SUPERVISOR Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SUPERVISOR> GetAll()
        {
            throw new NotImplementedException();
        }

        public SUPERVISOR Update(int id, SUPERVISOR supervisor)
        {
            throw new NotImplementedException();
        }
    }
}