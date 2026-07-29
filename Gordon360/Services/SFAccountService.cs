using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Extensions.System;
using Gordon360.Enums;
using System;
using Gordon360.Models.Salesforce;
using Microsoft.IdentityModel.Tokens;
using Gordon360.Exceptions;
using Gordon360.Static.Names;

namespace Gordon360.Services;


/// <summary>
/// Service Class that facilitates data transactions between the AccountsController and the Account database model.
/// </summary>
public class SFAccountService(CCTContext context, SFProfiles sfProcedures) : IAccountService
{

    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
    public async Task<AccountViewModel> GetAccountByID(string id)
    {
        if (id.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "id missing or empty" };
        var sfAccount = await sfProcedures.GetAccountByIdAsync(id) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" };;
        var account = (AccountViewModel) sfAccount;

        var cctAccount = context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        account = ComposeProfile(account, cctAccount);

        return account;
    }

    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ACCOUNT)]
    public async Task<IEnumerable<AccountViewModel>> GetAll()
    {
        var allAccounts = (IEnumerable<AccountViewModel>) await sfProcedures.GetAllAccountsAsync();
        return allAccounts;
    }

    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
    public async Task<AccountViewModel> GetAccountByEmail(string email)
    {
        if (email.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "email missing or empty" };
        var sfAccount = await sfProcedures.GetAccountByEmailAsync(email) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" };;
        var account = (AccountViewModel) sfAccount;

        var cctAccount = context.ACCOUNT.FirstOrDefault(x => x.email == email) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        account = ComposeProfile(account, cctAccount);

        return account;
    }

    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
    public async Task<AccountViewModel> GetAccountByUsername(string username)
    {
        if (username.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var sfAccount = await sfProcedures.GetAccountByAdUsernameAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" };;
        var account = (AccountViewModel) sfAccount;
        
        var cctAccount = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
        account = ComposeProfile(account, cctAccount);
        
        return account;
    }

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

    public async Task<IEnumerable<AdvancedSearchViewModel>> GetAccountsToSearch(List<string> accountTypes, IEnumerable<AuthGroup> authGroups, string? homeCity)
    {
        var accounts = (await sfProcedures.GetAllAccountsAsync())
            .Select(AdvancedSearchViewModel.FromAccount);
        if (!accountTypes.Contains("student")
            // Only students and FacStaff are authorized to search for students
            || !(authGroups.Contains(AuthGroup.FacStaff) || authGroups.Contains(AuthGroup.Student)))
        {
            accounts = accounts.Where(acc => acc.Type != "Student");
        }
        // Only Faculy and Staff can see Private students
        if (!authGroups.Contains(AuthGroup.FacStaff))
        {
            accounts = accounts.Where(acc => acc.KeepPrivate != "P");
        }
        // TODO: Implement
        // if (accountTypes.Contains("facstaff"))
        // {
        //     accounts = accounts.Where(acc => acc.ActiveAccount == true);
        // }
        if (accountTypes.Contains("alumni"))
        {
            accounts = accounts.Where(acc => acc.ShareName != "N");
        }
        // Do not indirectly reveal the address of facstaff and alumni who have requested to keep it private.
        if (!string.IsNullOrEmpty(homeCity))
        {
            accounts = accounts.Where(acc => acc.KeepPrivate == "0");
            accounts = accounts.Where(acc => acc.ShareAddress != "N");
        }

        return accounts;
    }

    public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoAsync()
    {
        var allAccounts = await sfProcedures.GetBasicInfo() ?? throw new ResourceNotFoundException() {ExceptionMessage = "No accounts found!"};
        var result = allAccounts.Select(BasicInfoViewModel.FromAccount);
        return result;
    }

    public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync()
    {
        var allAccountsExceptAlumni = await sfProcedures.GetBasicInfo(alumni: false) ?? throw new ResourceNotFoundException() {ExceptionMessage = "No accounts found!"};
        var result = allAccountsExceptAlumni.Select(BasicInfoViewModel.FromAccount);
        return result;
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
        static string Normalize(string name) =>
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

    /// <summary>
    /// Fill missing data in salesforce profile using CCT account.
    /// Should become obsolete once migration is complete.
    /// </summary>
    /// <param name="viewModel">Salesforce account</param>
    /// <param name="dbView">CCT Account</param>
    /// <returns>Filled-out profile</returns>
    private static AccountViewModel ComposeProfile(AccountViewModel viewModel, ACCOUNT dbView)
    {
        var account = viewModel;
        var cctAccount = (AccountViewModel) dbView;
        foreach (var prop in account.GetType().GetFields())
        {
            if (prop.GetValue(account) is null) prop.SetValue(account, prop.GetValue(cctAccount));
        }
        return account;
    }


}
