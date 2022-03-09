using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.CCT;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;

namespace Gordon360.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Student> StudentRepository { get; }
        IRepository<FacStaff> FacultyStaffRepository { get; }
        IRepository<Alumni> AlumniRepository { get; }
        IRepository<ACCOUNT> AccountRepository { get; }
        IRepository<EmergencyContact> EmergencyContactRepository { get; }
        IRepository<ACT_INFO> ActivityInfoRepository { get; }
        IRepository<CM_SESSION_MSTR> SessionRepository { get; }
        IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository { get; }
        IRepository<MEMBERSHIP> MembershipRepository { get; }
        IRepository<Mailboxes> MailboxRepository { get; }
        IRepository<MYSCHEDULE> MyScheduleRepository { get; }
        IRepository<PART_DEF> ParticipationRepository { get; }
        IRepository<REQUEST> MembershipRequestRepository { get; }
        IRepository<ADMIN> AdministratorRepository { get; }
        IRepository<Save_Rides> RideRepository { get; }
        IRepository<Save_Bookings> BookingRepository { get; }
        IRepository<Slider_Images> SliderRepository { get; }
        IRepository<CUSTOM_PROFILE> ProfileCustomRepository { get; }
        IRepository<ChapelEvent> ChapelEventRepository { get; }
        IRepository<DiningInfo> DiningInfoRepository { get; }
        IRepository<ERROR_LOG> ErrorLogRepository { get; }
        IRepository<Schedule_Control> ScheduleControlRepository { get; }
        IRepository<Health_Status> WellnessRepository { get; }

        // Note -- Only use this repository to call SQL Stored Procedures
        IRepository<ACTIVE_CLUBS_PER_SESS_IDResult> ActivityPerSessionRepository { get;  }
        IRepository<STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDEResult> StudentScheduleRepository { get; }
        IRepository<INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDEResult> FacultyScheduleRepository { get; }
        IRepository<VICTORY_PROMISE_BY_STUDENT_IDResult> VictoryPromiseByStudentIDRepository { get; }
        //IRepository<STUDENT_JOBS_PER_ID_NUM_Result> StudentEmploymentByStudentIDRepository { get; }

        IRepository<StudentNews> StudentNewsRepository { get; }
        IRepository<StudentNewsCategory> StudentNewsCategoryRepository { get; }
        IRepository<Clifton_Strengths> CliftonStrengthsRepository { get; }
    }
}