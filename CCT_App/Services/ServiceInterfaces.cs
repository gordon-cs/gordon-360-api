using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Models.ViewModels;

namespace CCT_App.Services
{
    public interface IAccountService
    {
        AccountViewModel Get(string id);
        IEnumerable<AccountViewModel> GetAll();
    }

    public interface IActivityService
    {
        ActivityViewModel Get(string id);
        IEnumerable<ActivityViewModel> GetAll();
        IEnumerable<SupervisorViewModel> GetSupervisorsForActivity(string id);
        IEnumerable<MembershipViewModel> GetLeadersForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id);
    }

    public interface ISessionService
    {
        CM_SESSION_MSTR Get(string id);
        IEnumerable<CM_SESSION_MSTR> GetAll();
        IEnumerable<ACT_CLUB_DEF> GetActivitiesForSession(string id);
        CM_SESSION_MSTR GetCurrentSession();
    }

    public interface IFacultyService
    {
        FacultyViewModel Get(string id);
        IEnumerable<FacultyViewModel> GetAll();
    }

    public interface IJenzibarActivityService
    {
        JNZB_ACTIVITIES Get(int id);
        IEnumerable<JNZB_ACTIVITIES> GetAll();
    }

    public interface IMembershipService
    {
        MembershipViewModel Get(int id);
        IEnumerable<MembershipViewModel> GetAll();
        Membership Add(Membership membership);
        Membership Update(int id, Membership membership);
        Membership Delete(int id);   
    }

    public interface IParticipationService
    {
        PART_DEF Get(string id);
        IEnumerable<PART_DEF> GetAll();
    }

    public interface IStaffService
    {
        Staff Get(string id);
        IEnumerable<Staff> GetAll();
    }

    public interface IStudentService
    {
        StudentViewModel Get(string id);
        IEnumerable<StudentViewModel> GetAll();
        IEnumerable<MembershipViewModel> GetActivitiesForStudent(string id);
    }

    public interface ISupervisorService
    {
        SUPERVISOR Get(int id);
        IEnumerable<SUPERVISOR> GetAll();
        SUPERVISOR Add(SUPERVISOR supervisor);
        SUPERVISOR Update(int id, SUPERVISOR supervisor);
        SUPERVISOR Delete(int id);
    }

    public interface IMembershipRequestService
    {
        Request Get(int id);
        IEnumerable<Request> GetAll();
        Request Add(Request membershipRequest);
        Request Update(int id, Request membershipRequest);
        Request Delete(int id);
    }
}