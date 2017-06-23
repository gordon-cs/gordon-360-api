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
        private IRepository<student_temp> _StudentTempRepository;
        private IRepository<facstaff> _FacultyStaffRepository;
        private IRepository<alumni> _AlumniRepository;
        private IRepository<ACCOUNT> _AccountRepository;
        private IRepository<ACT_CLUB_DEF> _ActivityRepository;
        private IRepository<CM_SESSION_MSTR> _SessionRepository;
        private IRepository<Faculty> _FacultyRepository;
        private IRepository<JNZB_ACTIVITIES> _JenzibarActvityRepository;
        private IRepository<MEMBERSHIP> _MembershipRepository;
        private IRepository<PART_DEF> _ParticipationRepository;
        private IRepository<Staff> _StaffRepository;
        private IRepository<Student> _StudentRepository;
        private IRepository<SUPERVISOR> _SupervisorRepository;
        private IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> _ActivityPerSessionRepository;
        private IRepository<REQUEST> _MembershipRequestRepository;
        private IRepository<ACT_INFO> _ActivityInfoRepository;
        private IRepository<ADMIN> _AdministratorRepository;
        private IRepository<C360_SLIDER> _SliderRepository;
        private IRepository<PROFILE_CUSTOM> _ProfileRepository;

        private CCTEntities1 _context;

        public UnitOfWork()
        {
            _context = new CCTEntities1();
        }
        public IRepository<student_temp> StudentTempRepository
        {
            get { return _StudentTempRepository ?? (_StudentTempRepository = new GenericRepository<student_temp>(_context)); }
        }
        public IRepository<facstaff> FacultyStaffRepository
        {
            get { return _FacultyStaffRepository ?? (_FacultyStaffRepository = new GenericRepository<facstaff>(_context)); }
        }
        public IRepository<alumni> AlumniRepository
        {
            get { return _AlumniRepository ?? (_AlumniRepository = new GenericRepository<alumni>(_context)); }
        }
        public IRepository<ACCOUNT> AccountRepository
        {
            get { return _AccountRepository ?? (_AccountRepository = new GenericRepository<ACCOUNT>(_context)); }
        }
        public IRepository<ACT_CLUB_DEF> ActivityRepository
        {
            get { return _ActivityRepository ?? (_ActivityRepository = new GenericRepository<ACT_CLUB_DEF>(_context)); }
        }
        public IRepository<ACT_INFO> ActivityInfoRepository
        {
            get { return _ActivityInfoRepository ?? (_ActivityInfoRepository = new GenericRepository<ACT_INFO>(_context)); }
        }
        public IRepository<CM_SESSION_MSTR> SessionRepository
        {
            get { return _SessionRepository ?? (_SessionRepository = new GenericRepository<CM_SESSION_MSTR>(_context)); }
        }
        public IRepository<Faculty> FacultyRepository
        {
            get { return _FacultyRepository ?? (_FacultyRepository = new GenericRepository<Faculty>(_context)); }
        }
        public IRepository<JNZB_ACTIVITIES> JenzibarActvityRepository
        {
            get { return _JenzibarActvityRepository ?? (_JenzibarActvityRepository = new GenericRepository<JNZB_ACTIVITIES>(_context)); }
        }
        public IRepository<MEMBERSHIP> MembershipRepository
        {
            get { return _MembershipRepository ?? (_MembershipRepository = new GenericRepository<MEMBERSHIP>(_context)); }
        }
        public IRepository<PART_DEF> ParticipationRepository
        {
            get { return _ParticipationRepository ?? (_ParticipationRepository = new GenericRepository<PART_DEF>(_context)); }
        }
        public IRepository<Staff> StaffRepository
        {
            get { return _StaffRepository ?? (_StaffRepository = new GenericRepository<Staff>(_context)); }
        }
        public IRepository<Student> StudentRepository
        {
            get { return _StudentRepository ?? (_StudentRepository = new GenericRepository<Student>(_context)); }
        }
        public IRepository<SUPERVISOR> SupervisorRepository
        {
            get { return _SupervisorRepository ?? (_SupervisorRepository = new GenericRepository<SUPERVISOR>(_context)); }
        }

        IRepository<ACTIVE_CLUBS_PER_SESS_ID_Result> IUnitOfWork.ActivityPerSessionRepository
        {
            get
            {
                 return _ActivityPerSessionRepository ?? (_ActivityPerSessionRepository = new GenericRepository<ACTIVE_CLUBS_PER_SESS_ID_Result>(_context)); 
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

        public IRepository<PROFILE_CUSTOM> ProfileRepository
        {
            get { return _ProfileRepository ?? (_ProfileRepository = new GenericRepository<PROFILE_CUSTOM>(_context)); }
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