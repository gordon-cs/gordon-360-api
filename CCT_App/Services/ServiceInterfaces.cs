using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;

namespace CCT_App.Services
{
    public interface IAccountService
    {
        ACCOUNT Get(string id);
        IEnumerable<ACCOUNT> GetAll();
    }

    public interface IActivityService
    {
        ACT_CLUB_DEF Get(string id);
        IEnumerable<ACT_CLUB_DEF> GetAll();
        SUPERVISOR GetSupervisorForActivity(string id);
    }

    public interface ISessionService
    {
        CM_SESSION_MSTR Get(string id);
        IEnumerable<CM_SESSION_MSTR> GetAll();
        IEnumerable<ACTIVE_CLUBS_PER_SESS_ID_Result> GetActivitiesForSession(string id);
    }

    public interface IFacultyService
    {
        Faculty Get(string id);
        IEnumerable<Faculty> GetAll();
    }

    public interface IJenzibarActivityService
    {
        JNZB_ACTIVITIES Get(int id);
        IEnumerable<JNZB_ACTIVITIES> GetAll();
    }

    public interface IMembershipService
    {
        Membership Get(int id);
        IEnumerable<Membership> GetAll();
        Membership Add(Membership membership);
        Membership Update(int id, Membership membership);
        Membership Delete(int id);   
    }

    public interface IRoleService
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
        Student Get(string id);
        IEnumerable<Student> GetAll();
        IEnumerable<ACT_CLUB_DEF> GetActivitiesForStudent(string id);
        Student Add(Student student);
        Student Update(string id, Student student);
        Student Delete(string id);
    }

    public interface ISupervisorService
    {
        SUPERVISOR Get(int id);
        IEnumerable<SUPERVISOR> GetAll();
        SUPERVISOR Add(SUPERVISOR supervisor);
        SUPERVISOR Update(int id, SUPERVISOR supervisor);
        SUPERVISOR Delete(int id);
    }
}