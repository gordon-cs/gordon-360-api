using Gordon360.Enums;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace Gordon360.Authorization;

/// <summary>
/// Utility class for fine-grained authorization checks
/// 
/// The "CanISee..." and "VisibleToMe..." methods encapsulate rules about whether
/// a requesting user can even find a user (CanISee...) and if so, what fields they 
/// are allowed to see (VisibleToMe...).
/// </summary>
public class AuthUtils
{
    static PrincipalContext Context => new(ContextType.Domain);

    /// <summary>
    /// Get the username of the authenticated user
    /// </summary>
    /// <param name="User">The ClaimsPrincipal representing the user's authentication claims</param>
    /// <returns>Username of the authenticated user</returns>
    public static string GetUsername(ClaimsPrincipal User)
    {
        return User.FindFirstValue(ClaimTypes.Upn).Split("@")[0];
    }

    public static IEnumerable<AuthGroup> GetGroups(ClaimsPrincipal User)
    {
        return User.Claims
            .Where(x => x.Type == "groups")
            .Select(g => AuthGroupEnum.FromString(g.Value))
            .OfType<AuthGroup>();
    }

    public static bool UserIsInGroup(ClaimsPrincipal User, AuthGroup group)
    {
        return GetGroups(User).Contains(group);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Program only runs on Windows")]
    public static IEnumerable<AuthGroup> GetGroups(string userName)
    {
        UserPrincipal user = UserPrincipal.FindByIdentity(Context, userName);

        if (user == null)
        {
            return Enumerable.Empty<AuthGroup>();
        }
        try
        {
            return user.GetAuthorizationGroups().Where(g => g is GroupPrincipal)
                           .Select(g => AuthGroupEnum.FromString(g.SamAccountName))
                           .OfType<AuthGroup>();
        }
        catch (NoMatchingPrincipalException)
        {
            return Enumerable.Empty<AuthGroup>();
        }
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for students.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the 
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for students, and false otherwise.</returns>
    public static bool CanISeeStudents(IEnumerable<AuthGroup> viewerGroups)
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
    public static bool CanISeeStudentSchedule(IEnumerable<AuthGroup> viewerGroups)
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
    public static bool CanISeeThisStudent(IEnumerable<AuthGroup> viewerGroups, StudentProfileViewModel? student)
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
    public static bool CanISeeFacstaff(IEnumerable<AuthGroup> viewerGroups)
    {
        return true;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for alumni.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the 
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for alumni, and false otherwise.</returns>
    public static bool CanISeeAlumni(IEnumerable<AuthGroup> viewerGroups)
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
    public static bool CanISeeAlumniSchedule(IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff);
    }
}
