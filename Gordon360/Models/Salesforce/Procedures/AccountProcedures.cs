using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class AccountProcedures(SalesforceContext context)
{
    private readonly SalesforceContext context = context;

    private const string SoqlTemplate = @"
        SELECT gc_Jenz_ID__c, FirstName, LastName, gc_University_Email__c
            FROM Account";

    /// <summary>
    /// Gets all accounts, sorted alphabetically by last name
    /// </summary>
    /// <returns></returns>
    public async Task<List<Account>> GetAllAccountsAsync()
    {
        var response = await context.SoqlQuery<Account>(SoqlTemplate, order: "LastName DESC NULLS LAST");

        return response.records;
    }

    /// <summary>
    /// Gets the account associated with a given ID number
    /// </summary>
    /// <param name="id">8-digit student ID number</param>
    /// <returns>Account associated with ID number</returns>
    public async Task<Account?> GetAccountByIdAsync(string id)
    {
        var response = await context.SoqlQuery<Account>(SoqlTemplate, where: $"gc_Jenz_ID__c = {id}", limit_n: 1);
        return response.records.FirstOrDefault();
    }

    /// <summary>
    /// Gets the account associated with a given email
    /// </summary>
    /// <param name="email">email address</param>
    /// <returns>Account associated with email</returns>
    public async Task<Account?> GetAccountByEmailAsync(string email)
    {
        var response = await context.SoqlQuery<Account>(SoqlTemplate, where: $"gc_University_Email__c = {email}", limit_n: 1);
        return response.records.FirstOrDefault();
    }

    /// <summary>
    /// Gets the account associated with a given email
    /// NOTE: This method is the same as GetAccountByEmailAsync,
    /// but adds "@gordon.edu" to the end of the username. Behaviour
    /// may be strange.
    /// </summary>
    /// <param name="adUsername">email address</param>
    /// <returns>Account associated with email</returns>
    public async Task<Account?> GetAccountByAdUsernameAsync(string adUsername)
    {
        var response = await context.SoqlQuery<Account>(SoqlTemplate, where: $"gc_University_Email__c = {adUsername}@gordon.edu", limit_n: 1);
        return response.records.FirstOrDefault();
    }
}