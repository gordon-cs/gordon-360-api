using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;

namespace Gordon360.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Student> StudentRepository { get; }
        IRepository<FacStaff> FacultyStaffRepository { get; }
        IRepository<Alumni> AlumniRepository { get; }
        IRepository<ACCOUNT> AccountRepository { get; }
        IRepository<ACT_INFO> ActivityInfoRepository { get; }
        IRepository<CM_SESSION_MSTR> SessionRepository { get; }
        IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository { get; }
        IRepository<MEMBERSHIP> MembershipRepository { get; }
        
        IRepository<MYSCHEDULE> MyScheduleRepository { get; }
        IRepository<PART_DEF> ParticipationRepository { get; }
        IRepository<SUPERVISOR> SupervisorRepository { get; }
        IRepository<REQUEST> MembershipRequestRepository { get; }
        IRepository<ADMIN> AdministratorRepository { get; }
        IRepository<Save_Rides> RideRepository { get; }
        IRepository<Save_Bookings> BookingRepository { get; }
        IRepository<C360_SLIDER> SliderRepository { get; }
        IRepository<CUSTOM_PROFILE> ProfileCustomRepository { get; }
        IRepository<ChapelEvent> ChapelEventRepository { get; }
        IRepository<DiningInfo> DiningInfoRepository { get; }
        IRepository<ERROR_LOG> ErrorLogRepository { get; }
        IRepository<Schedule_Control> ScheduleControlRepository { get; }

        // Note -- Only use this repository to call SQL Stored Procedures
        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> ActivityPerSessionRepository { get;  }
        IRepository<STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> StudentScheduleRepository { get; }
        IRepository<INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> FacultyScheduleRepository { get; }
        IRepository<VICTORY_PROMISE_BY_STUDENT_ID_Result> VictoryPromiseByStudentIDRepository { get; }
        //IRepository<STUDENT_JOBS_PER_ID_NUM_Result> StudentEmploymentByStudentIDRepository { get; }
        IRepository<StudentNewsViewModel> StudentNewsRepository { get; }
        IRepository<StudentNewsCategoryViewModel> StudentNewsCategoryRepository { get; }

        bool Save();


    }
}