using Gordon360.Authorization;
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

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the AccountsController and the Account database model.
/// </summary>
public class AccountService : IAccountService
{
    private readonly CCTContext context;
    private readonly IAcademicTermService academicTermService;

    public AccountService(CCTContext context, IAcademicTermService academicTermService)
    {
        this.context = context;
        this.academicTermService = academicTermService;
    }

    /// <summary>
    /// Fetches a single account record whose id matches the id provided as an argument
    /// </summary>
    /// <param name="id">The person's gordon id</param>
    /// <returns>AccountViewModel if found, null if not found</returns>
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
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
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ACCOUNT)]
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

    // ... other methods remain unchanged

    /// <summary>
    /// Get basic info for all accounts except alumni
    /// </summary>
    /// <returns>BasicInfoViewModel of all accounts except alumni</returns>
    public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync()
    {
        var basicInfo = await context.Procedures.ALL_BASIC_INFO_NOT_ALUMNIAsync();

        var usernames = basicInfo.Select(b => b.Username).ToList();

        var involvementLookup = context.MembershipView
            .Where(m => usernames.Contains(m.Username) && m.Privacy != true)
            .GroupBy(m => m.Username)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => m.ActivityDescription).Distinct().ToList()
            );

        return basicInfo.Select(b => new BasicInfoViewModel
        {
            FirstName = b.firstname,
            LastName = b.lastname,
            Nickname = b.Nickname,
            UserName = b.Username,
            Involvements = involvementLookup.TryGetValue(b.Username, out var involvements) ? involvements : null
        });
    }

    public async Task<IEnumerable<string>> GetActiveInvolvementNamesForCurrentTermAsync()
    {
        var currentTerm = await academicTermService.GetCurrentTermAsync();

        if (currentTerm == null || string.IsNullOrWhiteSpace(currentTerm.TermCode))
        {
            return Enumerable.Empty<string>();
        }

        var results = await context.Procedures.ACTIVE_CLUBS_PER_SESS_IDAsync(currentTerm.TermCode);

        return results
            .Select(r => r.ACT_DESC)

            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .OrderBy(name => name);
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
