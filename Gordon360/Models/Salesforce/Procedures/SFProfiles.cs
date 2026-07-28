using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Gordon360.Exceptions;

namespace Gordon360.Models.Salesforce;

public class SFProfiles(ISalesforceContext context)
{
    private const string SoqlTemplate = """
        SELECT 
            Name,
            Student_Id__pc,
            PersonEmail,
            PersonTitle,
            FirstName,
            MiddleName,
            LastName,
            FormerLastName__pc,
            Preferred_First_Name_Formula__pc,
            PersonGenderIdentity,
            AD_Username__pc,
            Suffix__pc,
            gc_Current_Student__c,
            gc_Current_Faculty__pc,
            gc_Current_Staff__pc,
            gc_is_Current_Alumni__c,
            gc_Resident_Commuter__pc,
            (
                SELECT 
                    Name,
                    LearningProgramPlan.LearningProgram.Type__c,
                    LearningProgramPlan.LearningProgram.Name,
                    LearningProgramPlan.LearningProgram.gc_Jenz_Major_Minor_Code__c,
                    Status
                FROM LearnerPrograms
            ),
            (
                SELECT StartDate, Position, EmploymentStatus
                FROM PersonEmployments
                ORDER BY StartDate DESC
            ),
            (
                SELECT
                    Name,
                    Description,
                    Status,
                    RoleType
                FROM Persons
            ),
            (
                SELECT 
                    Name,
                    Academic_Term__r.Name,

                    PhoneNumber,

                    Street,
                    City,
                    State,
                    PostalCode,
                    Country,
                    StateCode,
                    CountryCode,
                    gc_On_Campus_Location__r.Name,
                    gc_On_Campus_Location__r.ParentLocation.Name,
                    gc_On_Campus_Location__r.gc_Jenz_Building_Code__c,
                    gc_On_Campus_Location__r.gc_Jenz_Room_Code__c,
                    gc_Status__c,
                    AddressType
                FROM ContactPointAddresses
                WHERE (gc_Status__c = 'Current') AND ((AddressType = 'On-Campus') OR (AddressType = 'Home'))
            ),
            (
                SELECT 
                    Name,
                    Phone,
                    gc_Preferred_Class__c,
                    gc_Current_Positions__c,
    
                    MaritalStatus,
                    (
                        SELECT
                            Name,
                            PartyRoleRelation.Name,
                            RelatedContact.Name
                        FROM CCRContacts
                    )
                FROM Contacts
            )
        FROM Account
        """;

    /// <summary>
    /// Gets basic info for all accounts for search preview
    /// </summary>
    /// <param name="alumni">Whether or not to include alumni in the search</param>
    /// <returns>Minimal info about all accounts</returns>
    public async Task<IEnumerable<Account>> GetBasicInfo(bool alumni = true)
    {
        List<Account> result = [];

        SFQueryResult<Account>? response = await context.SoqlQuery<Account>(
           "SELECT FirstName, LastName, AD_Username__pc, Preferred_First_Name_Formula__pc, FormerLastName__pc " +
           "FROM Account",
           where: "IsPersonAccount = true" + (alumni ? "" : " AND gc_is_Current_Alumni__c = false"));

        while (!(response?.done ?? true) && response?.nextRecordsUrl is not null)
        {
            response = await context.GetNext<Account>(response!.nextRecordsUrl);
            result.AddRange(response?.records ?? []);
        }

        return result;
    }

    /// <summary>
    /// Gets the birthday of the given user
    /// </summary>
    /// <param name="username">AD username of the desired user</param>
    /// <returns>User's birthday, or null if not found or authorized</returns>
    public async Task<DateTime> GetBirthday(string username)
    {
        if (username.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "No user given!" };
        var response = await context.SoqlQuery<Account>("SELECT PersonBirthdate FROM Account",
                where: $"AD_Username__pc = '{username}'", limit_n: 1);

        var birthday = response?.records?.FirstOrDefault();

        return birthday == null ? DateTime.Now : DateTime.ParseExact(birthday.PersonBirthdate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets all accounts, sorted alphabetically by last name
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        List<Account> result = [];
        var response = await context.SoqlQuery<Account>(SoqlTemplate, order: "LastName DESC NULLS LAST");

        while (!(response?.done ?? true) && response?.nextRecordsUrl is not null)
        {
            response = await context.GetNext<Account>(response!.nextRecordsUrl);
            result.AddRange(response?.records ?? []);
        }

        return result;
    }

    /// <summary>
    /// Gets the account associated with a given ID number
    /// </summary>
    /// <param name="id">8-digit student ID number</param>
    /// <returns>Account associated with ID number</returns>
    public async Task<Account?> GetAccountByIdAsync(string id)
    {
        if (id.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "No id given!" };
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
        if (email.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "No email given!" };
        var response = await context.SoqlQuery<Account>(SoqlTemplate, where: $"gc_University_Email__c = {email}", limit_n: 1);
        return response.records.FirstOrDefault();
    }

    /// <summary>
    /// Gets the account associated with a given AD Username
    /// TODO: Due to full2 not being fully populated, this method also accepts a person's name
    /// </summary>
    /// <param name="adUsername">Active Directory username</param>
    /// <returns>Account associated with AD username</returns>
    public async Task<Account?> GetAccountByAdUsernameAsync(string adUsername)
    {
        if (adUsername.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "No username given!" };
        var whereString = $"RecordType.Name = 'Person Account' AND (AD_Username__pc = '{adUsername}' OR Name = '{adUsername}')";
        var response = await context.SoqlQuery<Account>(SoqlTemplate, where: whereString, limit_n: 1);
        return response.records.FirstOrDefault();
    }

    /// <summary>
    /// Gets a given account's mailbox combination
    /// </summary>
    /// <param name="adUsername">Active Directory username</param>
    /// <returns>Mailbox__c with only the combination field filled</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    public async Task<Mailbox__c?> GetMailboxCombinationAsync(string adUsername)
    {
        if (adUsername.IsNullOrEmpty()) throw new ResourceNotFoundException() { ExceptionMessage = "No username given!" };
        var whereString = $"gc_Assigned_Person__r.Account.AD_Username__pc  = '{adUsername}'";
        var response = await context.SoqlQuery<Mailbox__c>("SELECT gc_Combination__c FROM Mailbox__c", where: whereString, limit_n: 1);
        return response.records.FirstOrDefault();
    }
}