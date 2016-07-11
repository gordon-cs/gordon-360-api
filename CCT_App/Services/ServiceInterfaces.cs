using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;

/// <summary>
/// Namespace with all the Service Interfaces that are to be implemented. I don't think making this interface is required, the services can work find on their own.
/// However, building the interfaces first does give a general sense of structure to their implementations. A certian cohesiveness :p.
/// </summary>
namespace Gordon360.Services
{

    public interface IAccountService
    {
        AccountViewModel Get(string id);
        IEnumerable<AccountViewModel> GetAll();
    }

    public interface IActivityService
    {
        ActivityInfoViewModel Get(string id);
        IEnumerable<ActivityInfoViewModel> GetAll();
        IEnumerable<SupervisorViewModel> GetSupervisorsForActivity(string id);
        IEnumerable<MembershipViewModel> GetLeadersForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id);
    }

    public interface IActivityInfoService
    {
        ActivityInfoViewModel Get(string id);
        IEnumerable<ActivityInfoViewModel> GetAll();
    }

    public interface ISessionService
    {
        SessionViewModel Get(string id);
        IEnumerable<SessionViewModel> GetAll();
        IEnumerable<ActivityInfoViewModel> GetActivitiesForSession(string id);
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
        ParticipationViewModel Get(string id);
        IEnumerable<ParticipationViewModel> GetAll();
    }

    public interface IStaffService
    {
        StaffViewModel Get(string id);
        IEnumerable<StaffViewModel> GetAll();
    }

    public interface IStudentService
    {
        StudentViewModel Get(string id);
        IEnumerable<StudentViewModel> GetAll();
        IEnumerable<MembershipViewModel> GetActivitiesForStudent(string id);
    }

    public interface ISupervisorService
    {
        SupervisorViewModel Get(int id);
        IEnumerable<SupervisorViewModel> GetAll();
        SUPERVISOR Add(SUPERVISOR supervisor);
        SUPERVISOR Update(int id, SUPERVISOR supervisor);
        SUPERVISOR Delete(int id);
    }

    public interface IMembershipRequestService
    {
        MembershipRequestViewModel Get(int id);
        IEnumerable<MembershipRequestViewModel> GetAll();
        Request Add(Request membershipRequest);
        Request Update(int id, Request membershipRequest);
        Request Delete(int id);
    }
}