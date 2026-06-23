using Gordon360.Enums;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace Gordon360.Authorization;
public static class AuthGroupExtensions
{
    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for students.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for students, and false otherwise.</returns>
    public static bool CanISeeStudents(this IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff) ||
               viewerGroups.Contains(AuthGroup.Student);
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// course schedule information for students.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// schedule information for students, and false otherwise.</returns>
    public static bool CanISeeStudentSchedule(this IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.Advisors);
    }


    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for this particular student.  Some students are not shown
    /// because of FERPA protections.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <param name="student">Profile data for the student whose information
    /// is being requested.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for this student, and false otherwise.</returns>
    public static bool CanISeeThisStudent(this IEnumerable<AuthGroup> viewerGroups, StudentProfileViewModel? student)
    {
        if (!CanISeeStudents(viewerGroups))
        {
            return false;
        }

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff))
        {
            return true;
        }
        if (viewerGroups.Contains(AuthGroup.Student))
        {
            return (student == null) ? false : student.KeepPrivate != "Y" && student.KeepPrivate != "P";
        }
        return false;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for faculty and staff (facstaff).</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for facstaff, and false otherwise.</returns>
    public static bool CanISeeFacstaff(this IEnumerable<AuthGroup> viewerGroups)
    {
        return true;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for alumni.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for alumni, and false otherwise.</returns>
    public static bool CanISeeAlumni(this IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff) ||
               viewerGroups.Contains(AuthGroup.Alumni);
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// course schedule information for alumni.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// course schedule information for alumni, and false otherwise.</returns>
    public static bool CanISeeAlumniSchedule(this IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff);
    }

    /// <summary>Restrict info about a student to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.  Note that 
    /// this takes FERPA restrictions into account in determining whether this student
    /// is visible to the requesting user.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <param name="student">Profile data for the student whose information
    /// is being requested.</param>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about students.</returns>
    /// 
    public static object? VisibleToMeStudent(this IEnumerable<AuthGroup> viewerGroups, StudentProfileViewModel? student)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff))
        {
            return student;
        }
        else if (CanISeeThisStudent(viewerGroups, student))
        {
            return (student == null) ? null : student;
        }
        return null;
    }

    /// <summary>Restrict info about a facstaff person to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <param name="facstaff">Profile data for the facstaff member whose information
    /// is being requested.</param>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about facstaff.</returns>
    /// 
    public static object? VisibleToMeFacstaff(this IEnumerable<AuthGroup> viewerGroups, FacultyStaffProfileViewModel? facstaff)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return facstaff;
        }
        else if (CanISeeFacstaff(viewerGroups))
        {
            return (facstaff == null) ? null : (FacultyStaffProfileViewModel)facstaff;
        }
        return null;
    }

    /// <summary>Restrict info about an alumni person to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the
    /// user making the request.</param>
    /// <param name="alumni">Profile data for the alum whose information
    /// is being requested.</param>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about alumni.</returns>
    /// 
    public static object? VisibleToMeAlumni(this IEnumerable<AuthGroup> viewerGroups, AlumniProfileViewModel? alumni)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return alumni;
        }
        else if (CanISeeAlumni(viewerGroups))
        {
            return (alumni == null) ? null : (AlumniProfileViewModel)alumni;
        }
        return null;
    }
}
