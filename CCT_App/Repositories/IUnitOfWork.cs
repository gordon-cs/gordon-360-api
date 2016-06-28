using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;

namespace CCT_App.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<ACCOUNT> AccountRepository { get; }
        IRepository<ACT_CLUB_DEF> ActivityRepository { get; }
        IRepository<CM_SESSION_MSTR> SessionRepository { get; }
        IRepository<Faculty> FacultyRepository { get; }
        IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository { get; }
        IRepository<Membership> MembershipRepository { get; }
        IRepository<PART_DEF> RoleRepository { get; }
        IRepository<Staff> StaffRepository { get; }
        IRepository<Student> StudentRepository { get; }
        IRepository<SUPERVISOR> SupervisorRepository { get; }
        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> ActivityPerSessionRepository { get;  }

        bool Save();


    }
}