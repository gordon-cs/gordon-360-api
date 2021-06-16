using Gordon360.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // check college role for a user
        public string getCollegeRole(string username)
        {
            var userAccount = _unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username);

            if (userAccount == null)
            {
                // If user == ALUMNI
                if(_unitOfWork.AlumniRepository.Any(x => x.AD_Username == username))
                {
                    return Position.ALUMNI;
                }
            }

            var viewer = userAccount;
            string type = viewer.account_type;
            var id = viewer.gordon_id;
            bool isAdmin = _unitOfWork.AdministratorRepository.FirstOrDefault(x => x.ID_NUM.ToString() == id) != null;
            bool isPolice = viewer.is_police == 1;

            if (isAdmin)
            {
                type = Position.SUPERADMIN;
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
