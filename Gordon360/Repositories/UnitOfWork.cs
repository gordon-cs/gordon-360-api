using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using System.Data.Entity.Validation;
using System.Text;

namespace Gordon360.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Student> _StudentRepository;
        private IRepository<FacStaff> _FacultyStaffRepository;
        private IRepository<Alumni> _AlumniRepository;
        private IRepository<ACCOUNT> _AccountRepository;
        private IRepository<CM_SESSION_MSTR> _SessionRepository;
        private IRepository<JNZB_ACTIVITIES> _JenzibarActvityRepository;
        private IRepository<MEMBERSHIP> _MembershipRepository;
        private IRepository<INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> _FacultyScheduleRepository;
        private IRepository<STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> _StudentScheduleRepository;
        private IRepository<MYSCHEDULE> _MyScheduleRepository;
        private IRepository<RIDE> _RideRepository;
        private IRepository<PART_DEF> _ParticipationRepository;
        private IRepository<SUPERVISOR> _SupervisorRepository;
        private IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> _ActivityPerSessionRepository;
        private IRepository<VICTORY_PROMISE_BY_STUDENT_ID_Result> _VictoryPromiseByStudentIDRepository;
        private IRepository<STUDENT_JOBS_PER_ID_NUM_Result> _StudentEmploymentByStudentIDRepository;
        private IRepository<REQUEST> _MembershipRequestRepository;
        private IRepository<ACT_INFO> _ActivityInfoRepository;
        private IRepository<ADMIN> _AdministratorRepository;
        private IRepository<C360_SLIDER> _SliderRepository;
        private IRepository<CUSTOM_PROFILE> _ProfileCustomRepository;
        private IRepository<ChapelEvent> _ChapelEventRepository;
        private IRepository<DiningInfo> _DiningInfoRepository;
        private IRepository<ERROR_LOG> _ErrorLogRepository;
        private IRepository<Schedule_Control> _ScheduleControlRepository;

        private CCTEntities1 _context;

        public UnitOfWork()
        {
            _context = new CCTEntities1();
        }
        public IRepository<Student> StudentRepository
        {
            get { return _StudentRepository ?? (_StudentRepository = new GenericRepository<Student>(_context)); }
        }
        public IRepository<FacStaff> FacultyStaffRepository
        {
            get { return _FacultyStaffRepository ?? (_FacultyStaffRepository = new GenericRepository<FacStaff>(_context)); }
        }
        public IRepository<Alumni> AlumniRepository
        {
            get { return _AlumniRepository ?? (_AlumniRepository = new GenericRepository<Alumni>(_context)); }
        }
        public IRepository<ACCOUNT> AccountRepository
        {
            get { return _AccountRepository ?? (_AccountRepository = new GenericRepository<ACCOUNT>(_context)); }
        }
        public IRepository<ACT_INFO> ActivityInfoRepository
        {
            get { return _ActivityInfoRepository ?? (_ActivityInfoRepository = new GenericRepository<ACT_INFO>(_context)); }
        }
        public IRepository<CM_SESSION_MSTR> SessionRepository
        {
            get { return _SessionRepository ?? (_SessionRepository = new GenericRepository<CM_SESSION_MSTR>(_context)); }
        }
        public IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository
        {
            get { return _JenzibarActvityRepository ?? (_JenzibarActvityRepository = new GenericRepository<JNZB_ACTIVITIES>(_context)); }
        }
        public IRepository<MEMBERSHIP> MembershipRepository
        {
            get { return _MembershipRepository ?? (_MembershipRepository = new GenericRepository<MEMBERSHIP>(_context)); }
        }
        public IRepository<STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> StudentScheduleRepository
        {
            get { return _StudentScheduleRepository ?? (_StudentScheduleRepository = new GenericRepository<STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE_Result>(_context)); }
        }
        public IRepository<INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE_Result> FacultyScheduleRepository
        {
            get { return _FacultyScheduleRepository ?? (_FacultyScheduleRepository = new GenericRepository<INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE_Result>(_context)); }
        }
        public IRepository<MYSCHEDULE> MyScheduleRepository
        {
            get { return _MyScheduleRepository ?? (_MyScheduleRepository = new GenericRepository<MYSCHEDULE>(_context)); }
        }

        public IRepository<PART_DEF> ParticipationRepository
        {
            get { return _ParticipationRepository ?? (_ParticipationRepository = new GenericRepository<PART_DEF>(_context)); }
        }
        public IRepository<SUPERVISOR> SupervisorRepository
        {
            get { return _SupervisorRepository ?? (_SupervisorRepository = new GenericRepository<SUPERVISOR>(_context)); }
        }
        public IRepository<Schedule_Control> ScheduleControlRepository
        {
            get { return _ScheduleControlRepository ?? (_ScheduleControlRepository = new GenericRepository<Schedule_Control>(_context)); }
        }

        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> IUnitOfWork.ActivityPerSessionRepository
        {
            get
            {
                 return _ActivityPerSessionRepository ?? (_ActivityPerSessionRepository = new GenericRepository<ACTIVE_CLUBS_PER_SESS_ID_Result>(_context)); 
            }
        }
        IRepository<VICTORY_PROMISE_BY_STUDENT_ID_Result> IUnitOfWork.VictoryPromiseByStudentIDRepository
        {
            get
            {
                return _VictoryPromiseByStudentIDRepository ?? (_VictoryPromiseByStudentIDRepository = new GenericRepository<VICTORY_PROMISE_BY_STUDENT_ID_Result>(_context));
            }
        }
        IRepository<STUDENT_JOBS_PER_ID_NUM_Result> IUnitOfWork.StudentEmploymentByStudentIDRepository
        {
            get
            {
                return _StudentEmploymentByStudentIDRepository ?? (_StudentEmploymentByStudentIDRepository = new GenericRepository<STUDENT_JOBS_PER_ID_NUM_Result>(_context));
            }
        }
        public IRepository<REQUEST> MembershipRequestRepository
        {
            get { return _MembershipRequestRepository ?? (_MembershipRequestRepository = new GenericRepository<REQUEST>(_context)); }
        }

        public IRepository<ADMIN> AdministratorRepository
        {
            get { return _AdministratorRepository ?? (_AdministratorRepository = new GenericRepository<ADMIN>(_context));  }
        }

        public IRepository<C360_SLIDER> SliderRepository
        {
            get { return _SliderRepository ?? (_SliderRepository = new GenericRepository<C360_SLIDER>(_context));  }
        }
        public IRepository<ChapelEvent> ChapelEventRepository
        {
            get { return _ChapelEventRepository ?? (_ChapelEventRepository = new GenericRepository<ChapelEvent>(_context)); }
        }
        public IRepository<DiningInfo> DiningInfoRepository
        {
            get { return _DiningInfoRepository ?? (_DiningInfoRepository = new GenericRepository<DiningInfo>(_context)); }
        }
        public IRepository<CUSTOM_PROFILE> ProfileCustomRepository
        {
            get { return _ProfileCustomRepository ?? (_ProfileCustomRepository = new GenericRepository<CUSTOM_PROFILE>(_context)); }
        }
        public IRepository<ERROR_LOG> ErrorLogRepository
        {
            get { return _ErrorLogRepository ?? (_ErrorLogRepository = new GenericRepository<ERROR_LOG>(_context)); }
        }

        public IRepository<RIDE> RideRepository
        {
            get { return _RideRepository ?? (_RideRepository = new GenericRepository<RIDE>(_context)); }
        }

        public bool Save()
        {

            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            return true;

        }
    }
}