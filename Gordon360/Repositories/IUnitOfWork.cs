using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;

namespace Gordon360.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Student> StudentRepository { get; }
        IRepository<FacStaff> FacultyStaffRepository { get; }
        IRepository<Alumni> AlumniRepository { get; }
        IRepository<ACCOUNT> AccountRepository { get; }
        IRepository<ACT_CLUB_DEF_DELETE> ActivityRepository { get; }
        IRepository<ACT_INFO> ActivityInfoRepository { get; }
        IRepository<CM_SESSION_MSTR> SessionRepository { get; }
        IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository { get; }
        IRepository<MEMBERSHIP> MembershipRepository { get; }
        IRepository<PART_DEF> ParticipationRepository { get; }
        IRepository<SUPERVISOR> SupervisorRepository { get; }
        IRepository<REQUEST> MembershipRequestRepository { get; }
        IRepository<ADMIN> AdministratorRepository { get; }
        IRepository<C360_SLIDER> SliderRepository { get; }
        IRepository<CUSTOM_PROFILE> ProfileCustomRepository { get; }
        IRepository<ChapelEvent> ChapelEventRepository { get; }

        // Note -- Only use this repository to call SQL Stored Procedures
        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> ActivityPerSessionRepository { get;  }

        bool Save();


    }
}