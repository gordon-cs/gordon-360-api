using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;

namespace Gordon360.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<ACCOUNT> AccountRepository { get; }
        IRepository<ACT_CLUB_DEF> ActivityRepository { get; }
        IRepository<Activity_Info> ActivityInfoRepository { get; }
        IRepository<CM_SESSION_MSTR> SessionRepository { get; }
        IRepository<Faculty> FacultyRepository { get; }
        IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository { get; }
        IRepository<Membership> MembershipRepository { get; }
        IRepository<PART_DEF> ParticipationRepository { get; }
        IRepository<Staff> StaffRepository { get; }
        IRepository<Student> StudentRepository { get; }
        IRepository<SUPERVISOR> SupervisorRepository { get; }
        IRepository<Request> MembershipRequestRepository { get; }

        // Note -- Only use this repository to call SQL Stored Procedures
        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> ActivityPerSessionRepository { get;  }

        bool Save();


    }
}