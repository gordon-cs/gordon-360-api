using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using Gordon360.Static.Names;

namespace Gordon360.Services
{
    public class RoleCheckingService: IRoleCheckingService
    {
        private IUnitOfWork _unitOfWork;

        public RoleCheckingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string getCollegeRole(string username)
        {
            var viewer = _unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username);
            string type = viewer.account_type;
            var id = viewer.gordon_id;
            bool isAdmin = _unitOfWork.AdministratorRepository.FirstOrDefault(x => x.ID_NUM.ToString() == id) != null;
            bool isPolice = viewer.is_police == 1;


            if (isAdmin)
            {
                type = Position.GOD;
                return type;
            }
            else if (isPolice)
                type = Position.POLICE;
            else if (type == "STUDENT")
                type = Position.STUDENT;
            else if (type == "FACULTY" || type == "STAFF")
                type = Position.FACSTAFF;
            return type;
        }
    }
}