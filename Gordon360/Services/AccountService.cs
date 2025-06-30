﻿using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Extensions.System;
using Gordon360.Enums;
using System;
using Microsoft.Graph;

namespace Gordon360.Services;


/// <summary>
/// Service Class that facilitates data transactions between the AccountsController and 
/// the Account database model.  It also provides methods to enforce access restrictions
/// on user data.
/// 
/// The "CanISee..." and "VisibleToMe..." methods encapsulate rules about whether
/// a requesting user can even find a user (CanISee...) and if so, what fields they 
/// are allowed to see (VisibleToMe...).
/// </summary>
public class AccountService(CCTContext context) : IAccountService
{

    /// <summary>
    /// Fetches a single account record whose id matches the id provided as an argument
    /// </summary>
    /// <param name="id">The person's gordon id</param>
    /// <returns>AccountViewModel if found, null if not found</returns>
    [StateYourBusiness(operation = Static.Names.Operation.READ_ONE, resource = Resource.ACCOUNT)]
    public AccountViewModel GetAccountByID(string id)
    {
        var account = context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
        if (account == null)
        {
            // Custom Exception is thrown that will be cauth in the controller Exception filter.
            throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        }

        return account;
    }

    /// <summary>
    /// Fetches all the account records from storage.
    /// </summary>
    /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
    [StateYourBusiness(operation = Static.Names.Operation.READ_ALL, resource = Resource.ACCOUNT)]
    public IEnumerable<AccountViewModel> GetAll()
    {
        return (IEnumerable<AccountViewModel>)context.ACCOUNT; //Map the database model to a more presentable version (a ViewModel)
    }

    /// <summary>
    /// Fetches the account record with the specified email.
    /// </summary>
    /// <param name="email">The email address associated with the account.</param>
    /// <returns>the first account object which matches the email</returns>
    public AccountViewModel GetAccountByEmail(string email)
    {
        var account = context.ACCOUNT.FirstOrDefault(x => x.email == email);
        if (account == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        }

        return account;
    }

    /// <summary>
    /// Fetches the account record with the specified username.
    /// </summary>
    /// <param name="username">The AD username associated with the account.</param>
    /// <returns>the student account information</returns>
    public AccountViewModel GetAccountByUsername(string username)
    {
        var account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
        if (account == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        }

        return account;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for students.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the 
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for students, and false otherwise.</returns>
    public bool CanISeeStudents(IEnumerable<AuthGroup> viewerGroups)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff) ||
            viewerGroups.Contains(AuthGroup.Student))
        {
            return true;
        }
        return false;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// course schedule information for students.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the 
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// schedule information for students, and false otherwise.</returns>
    public bool CanISeeStudentSchedule(IEnumerable<AuthGroup> viewerGroups)
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
    public bool CanISeeThisStudent(IEnumerable<AuthGroup> viewerGroups, StudentProfileViewModel? student)
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
    public bool CanISeeFacstaff(IEnumerable<AuthGroup> viewerGroups)
    {
        return true;
    }

    /// <summary>Indicates whether a user making a request is authorized to see
    /// profile information for alumni.</summary>
    /// <param name="viewerGroups">The authentication groups associated with the 
    /// user making the request.</param>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for alumni, and false otherwise.</returns>
    public bool CanISeeAlumni(IEnumerable<AuthGroup> viewerGroups)
    {
        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff) ||
               viewerGroups.Contains(AuthGroup.Alumni);
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
    public object? VisibleToMeStudent(IEnumerable<AuthGroup> viewerGroups, StudentProfileViewModel? student)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff))
        {
            return student;
        }
        else if (CanISeeThisStudent(viewerGroups, student))
        {
            return (student == null) ? null : (PublicStudentProfileViewModel)student;
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
    public object? VisibleToMeFacstaff(IEnumerable<AuthGroup> viewerGroups, FacultyStaffProfileViewModel? facstaff)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return facstaff;
        }
        else if (CanISeeFacstaff(viewerGroups))
        {
            return (facstaff == null) ? null : (PublicFacultyStaffProfileViewModel)facstaff;
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
    public object? VisibleToMeAlumni(IEnumerable<AuthGroup> viewerGroups, AlumniProfileViewModel? alumni)
    {
        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return alumni;
        }
        else if (CanISeeAlumni(viewerGroups))
        {
            return (alumni == null) ? null : (PublicAlumniProfileViewModel)alumni;
        }
        return null;
    }

    /// <summary>
    /// Given a list of accounts, and search params, return all the accounts that match those search params.
    /// </summary>
    /// <param name="accounts">The accounts that should be searched, converted to an AdvancedSearchViewModel</param>
    /// <param name="firstname">The firstname search param</param>
    /// <param name="lastname">The lastname search param</param>
    /// <param name="major">The major search param</param>
    /// <param name="minor">The minor search param</param>
    /// <param name="hall">The hall search param</param>
    /// <param name="classType">The class type search param, e.g. 'Sophomore', 'Senior', 'Undergraduate Conferred'</param>
    /// <param name="preferredClassYear">The preferred class year search param</param>
    /// <param name="initialYear">The initial year range search param</param>
    /// <param name="finalYear">The final year range search param</param>
    /// <param name="homeCity">The home city search param</param>
    /// <param name="state">The state search param</param>
    /// <param name="country">The country search param</param>
    /// <param name="department">The department search param</param>
    /// <param name="building">The building search param</param>
    /// <param name="involvement">The involvement search param</param>
    /// <param name="gender">The gender search param</param>
    /// <returns>The accounts from the provided list that match the given search params</returns>
    public IEnumerable<AdvancedSearchViewModel> AdvancedSearch(
        IEnumerable<AdvancedSearchViewModel> accounts,
        string? firstname,
        string? lastname,
        string? major,
        string? minor,
        string? hall,
        string? classType,
        string? preferredClassYear,
        int? initialYear,
        int? finalYear,
        string? homeCity,
        string? state,
        string? country,
        string? department,
        string? building,
        string? involvement,
        string? gender)
    {
        // Accept common town abbreviations in advanced people search
        // East = E, West = W, South = S, North = N
        if (
            !string.IsNullOrEmpty(homeCity)
            && (
              homeCity.StartsWithIgnoreCase("e ") ||
              homeCity.StartsWithIgnoreCase("w ") ||
              homeCity.StartsWithIgnoreCase("s ") ||
              homeCity.StartsWithIgnoreCase("n ")
            )
          )
        {
            homeCity =
                homeCity
                    .Replace("e ", "east ")
                    .Replace("w ", "west ")
                    .Replace("s ", "south ")
                    .Replace("n ", "north ");
        }

        if (firstname is not null)
        {
            accounts = accounts.Where(a =>
                a.FirstName.StartsWithIgnoreCase(firstname)
                || a.NickName.StartsWithIgnoreCase(firstname)
                || a.Email.StartsWithIgnoreCase(firstname));
        }

        if (lastname is not null)
        {
            accounts = accounts.Where(a =>
                a.LastName.StartsWithIgnoreCase(lastname)
                || a.MaidenName.StartsWithIgnoreCase(lastname)
                || (!string.IsNullOrEmpty(a.Email) && a.Email.IndexOf('.') >= 0 
                    && a.Email.Split('.')[1].StartsWithIgnoreCase(lastname))
            );
        }

        if (major is not null)
        {
            accounts = accounts.Where(a =>
                a.Major1Description.EqualsIgnoreCase(major)
                || a.Major2Description.EqualsIgnoreCase(major)
                || a.Major3Description.EqualsIgnoreCase(major)
            );
        }

        if (minor is not null)
        {
            accounts = accounts.Where(a =>
                   a.Minor1Description.EqualsIgnoreCase(minor)
                || a.Minor2Description.EqualsIgnoreCase(minor)
                || a.Minor3Description.EqualsIgnoreCase(minor)
            );
        }

        if (hall is not null) accounts = accounts.Where(a => a.Hall.StartsWithIgnoreCase(hall));
        if (classType is not null) accounts = accounts.Where(a => a.Class.StartsWithIgnoreCase(classType));
        if (preferredClassYear is not null) accounts = accounts.Where(a => a.PreferredClassYear == (preferredClassYear));
        if ((initialYear is not null) && (finalYear is not null))
        {
            accounts = accounts.Where(a => a.PreferredClassYear != "");
            accounts = accounts.Where(a => Convert.ToInt32(a.PreferredClassYear) >= initialYear && Convert.ToInt32(a.PreferredClassYear) <= finalYear);
        }
        if (homeCity is not null) accounts = accounts.Where(a => a.HomeCity.StartsWithIgnoreCase(homeCity));
        if (state is not null) accounts = accounts.Where(a => a.HomeState.StartsWithIgnoreCase(state));
        if (country is not null) accounts = accounts.Where(a => a.Country.StartsWithIgnoreCase(country));
        if (department is not null) accounts = accounts.Where(a => a.OnCampusDepartment.StartsWithIgnoreCase(department));
        if (building is not null) accounts = accounts.Where(a => a.BuildingDescription.StartsWithIgnoreCase(building));
        if (involvement is not null)
        {
            var members = context.MembershipView.Where(mv => mv.ActivityDescription == involvement && mv.Privacy != true);
            accounts = accounts.Join(members, a => a.AD_Username, mv => mv.Username, (a, mv) => a).Distinct();
        }
        if (gender is not null) accounts = accounts.Where(a => a.Gender.StartsWithIgnoreCase(gender));

        return accounts.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);
    }

    /// <summary>
    /// Get the list of accounts a user can search, based on the types of accounts they want to search, their authorization, and whether they're searching sensitive info.
    /// </summary>
    /// <param name="accountTypes">A list of account types that will be searched: 'student', 'alumni', and/or 'facstaff'</param>
    /// <param name="authGroups">The authorization groups of the searching user, to decide what accounts they are permitted to search</param>
    /// <param name="homeCity">The home city search param, since it is considered sensitive info</param>
    /// <returns>The list of accounts that may be searched, converted to AdvancedSearchViewModels.</returns>
    public IEnumerable<AdvancedSearchViewModel> GetAccountsToSearch(List<string> accountTypes, IEnumerable<AuthGroup> authGroups, string? homeCity)
    {
        IEnumerable<Student> students = Enumerable.Empty<Student>();
        if (accountTypes.Contains("student")
            // Only students and FacStaff are authorized to search for students
            && (authGroups.Contains(AuthGroup.FacStaff) || authGroups.Contains(AuthGroup.Student)))
        {
            students = context.Student;
        }

        // Only Faculty and Staff can see Private students
        if (!authGroups.Contains(AuthGroup.FacStaff))
        {
            students = students.Where(s => (s.KeepPrivate != "Y" && s.KeepPrivate != "P"));
        }

        IEnumerable<FacStaff> facstaff = Enumerable.Empty<FacStaff>();
        if (accountTypes.Contains("facstaff"))
        {
            facstaff = context.FacStaff.Where(fs => fs.ActiveAccount == true);
        }

        IEnumerable<Alumni> alumni = Enumerable.Empty<Alumni>();
        if (accountTypes.Contains("alumni"))
        {
            alumni = context.Alumni.Where(a => a.ShareName != "N");
        }

        // Do not indirectly reveal the address of facstaff and alumni who have requested to keep it private.
        if (!string.IsNullOrEmpty(homeCity))
        {
            var homePrivacy = authGroups.Contains(AuthGroup.FacStaff)
                ? context.UserPrivacy_Settings.Where(a => (a.Field == "HomeCity" && a.Visibility != "Private"))
                : context.UserPrivacy_Settings.Where(a => (a.Field == "HomeCity" && a.Visibility != "Private" && a.Visibility != "FacStaff"));
            facstaff = facstaff.Join(homePrivacy,
                user => user.ID, privs => privs.gordon_id,
                (user, privs) => user //new { id = user.ID, visibility = privs.Visibility, }
            );
            alumni = alumni.Where(a => a.ShareAddress != "N");
        }

        return students.Select<Student, AdvancedSearchViewModel>(s => s)
            .UnionBy(facstaff.Select<FacStaff, AdvancedSearchViewModel>(fs => fs), a => a.AD_Username)
            .UnionBy(alumni.Select<Alumni, AdvancedSearchViewModel>(a => a), a => a.AD_Username);
    }

    /// <summary>
    /// Get basic info for all accounts
    /// </summary>
    /// <returns>BasicInfoViewModel of all accounts</returns>
    public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoAsync()
    {

        var basicInfo = await context.Procedures.ALL_BASIC_INFOAsync();
        return basicInfo.Select(
            b => new BasicInfoViewModel
            {
                FirstName = b.FirstName,
                LastName = b.LastName,
                Nickname = b.Nickname,
                UserName = b.UserName
            });
    }

    /// <summary>
    /// Get basic info for all accounts except alumni
    /// </summary>
    /// <returns>BasicInfoViewModel of all accounts except alumni</returns>
    public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync()
    {
        var basicInfo = await context.Procedures.ALL_BASIC_INFO_NOT_ALUMNIAsync();
        return basicInfo.Select(
            b => new BasicInfoViewModel
            {
                FirstName = b.firstname,
                LastName = b.lastname,
                Nickname = b.Nickname,
                UserName = b.Username
            });
    }

    public ParallelQuery<BasicInfoViewModel> Search(string searchString, IEnumerable<BasicInfoViewModel> accounts)
    {
        return accounts.AsParallel()
           .Select(account => (matchKey: account.MatchSearch(searchString), account))
           .Where(pair => pair.matchKey is not null)
           .OrderBy(pair => pair.matchKey)
           .Select(pair => pair.account);
    }

    public ParallelQuery<BasicInfoViewModel> Search(string firstName, string lastName, IEnumerable<BasicInfoViewModel> accounts)
    {
        string Normalize(string name) =>
            new string(name?.Where(char.IsLetterOrDigit).ToArray()).ToLower();


        var normalizedLastName = Normalize(lastName);

        return accounts.AsParallel()
            .Select(account =>
            {
                var matchKey = account.MatchSearch(firstName, normalizedLastName);
                return (matchKey, account);
            })
            .Where(pair => pair.matchKey is not null)
            .OrderBy(pair => pair.matchKey)
            .Select(pair => pair.account);
    }


}
