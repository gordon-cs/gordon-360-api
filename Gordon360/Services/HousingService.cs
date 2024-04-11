using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Static.Methods;
using Microsoft.EntityFrameworkCore;
using Gordon360.Models.webSQL.Context;
using System.Threading.Tasks;
using Gordon360.Models.webSQL.Models;
using Microsoft.Graph;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Identity.Client;

namespace Gordon360.Services;

public class HousingService(CCTContext context, IAccountService accountService) : IHousingService
{

    /// <summary>
    /// Calls a stored procedure that returns a row in the staff whitelist which has the given user id,
    /// if it is in the whitelist
    /// </summary>
    /// <param name="username"> The id of the person using the page </param>
    /// <returns> Whether or not the user is on the staff whitelist </returns>
    public bool CheckIfHousingAdmin(string username)
    {
        return AuthUtils.GetGroups(username).Contains(AuthGroup.HousingAdmin);
    }

    /// <summary>
    /// Deletes the application with given id,
    /// removing all rows that reference it.
    /// </summary>
    /// <param name="applicationID"> The id of the application to delete </param>
    /// <returns> Whether or not this was successful </returns>
    public bool DeleteApplication(int applicationID)
    {
        try
        {
            var result = context.Housing_Applications.Remove(new Housing_Applications { HousingAppID = applicationID });
            context.SaveChanges();
            return true;
        }
        catch
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The housing application could not be found." };
        }
    }

    /// <summary>
    /// Gets all names of apartment halls
    /// </summary>
    /// <returns> AN array of hall names </returns>
    public string[] GetAllApartmentHalls()
    {

        var hallsResult = context.Housing_Halls.Where(h => h.Type == "Apartment").Select(h => h.Name);
        if (hallsResult == null || !hallsResult.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The apartment halls could not be found." };
        }

        return hallsResult.ToArray();
    }

        /// <summary>
        /// Gets all names of apartment halls
        /// </summary>
        /// <returns> AN array of hall names </returns>
        public string[] GetAllTraditionalHalls()
        {

            var hallsResult = context.Housing_Halls.Where(h => h.Type == "Traditional").Select(h => h.Name);
            if (hallsResult == null || !hallsResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The traditional halls could not be found." };
            }

            return hallsResult.ToArray();
        }

    /// <summary>
    /// Calls a stored procedure that tries to get the id of an the application that a given user is 
    /// applicant on for a given session
    /// </summary>
    /// <param name="username"> The student username to look for </param>
    /// <param name="sess_cde"> Session for which the application would be </param>
    /// <returns> 
    /// The id of the application or 
    /// null if the user is not on an application for that session 
    /// </returns>
    public int? GetApplicationID(string username, string sess_cde)
    {
        return context.Housing_Applicants.Where(a => a.Username == username && a.SESS_CDE == sess_cde).Select(a => a.HousingAppID).FirstOrDefault();
    }

    /// <summary>
    /// Get the editor ID of a given application ID
    /// </summary>
    /// <param name="applicationID"> The application ID for which the editor ID would be </param>
    /// <returns>
    /// The id of the editor or
    /// null if the user is a member but not an editor of a given application
    /// </returns>
    public string GetEditorUsername(int applicationID)
    {
        return context.Housing_Applications.Where(a => a.HousingAppID == applicationID).Select(a => a.EditorUsername).FirstOrDefault();
    }

    /// <summary>
    /// Saves student housing info
    /// - first, it creates a new row in the applications table and inserts the username of the primary applicant and a timestamp
    /// - second, it retrieves the application id of the application with the information we just inserted (because
    /// the database creates the application ID so we have to ask it which number it generated for it)
    /// - third, it inserts each applicant into the applicants table along with the application ID so we know
    /// which application on which they are an applicant
    ///
    /// </summary>
    /// <param name="sess_cde"> The current session code </param>
    /// <param name="editorUsername"> The student username of the student who is declared to be the editor of this application (retrieved from the JSON from the front end) </param>
    /// <param name="apartmentApplicants"> Array of JSON objects providing apartment applicants </param>
    /// <param name="apartmentChoices"> Array of JSON objects providing apartment hall choices </param>
    /// <returns>Returns the application ID number if all the queries succeeded</returns>
    public int SaveApplication(string sess_cde, string editorUsername, List<ApartmentApplicantViewModel> apartmentApplicants, List<ApartmentChoiceViewModel> apartmentChoices)
    {
        if (GetApplicationID(editorUsername, sess_cde) != 0)
        {
            throw new ResourceCreationException() { ExceptionMessage = "An existing application ID was found for this user. Please use 'EditApplication' to update an existing application." };
        }

        // Save the application editor and time
        // If an existing application was not found for this editor, then insert a new application entry in the database
        var newAppResult = context.Housing_Applications.Add(new Housing_Applications { DateModified = DateTime.Now, EditorUsername = editorUsername });

        context.SaveChanges();

        if (newAppResult?.Entity == null || newAppResult?.Entity?.HousingAppID == 0)
        {
            throw new ResourceCreationException() { ExceptionMessage = "The application could not be saved." };
        }

        // Retrieve the application ID number of this new application
        int applicationID = newAppResult.Entity.HousingAppID;

        // Save applicant
        foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
        {
            var applicantResult = context.Housing_Applicants.Add(new Housing_Applicants { HousingAppID = applicationID, Username = applicant.Username, AprtProgram = applicant.OffCampusProgram ?? "", AprtProgramCredit = false, SESS_CDE = sess_cde });
            if (applicantResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = $"Applicant {applicant.Username} could not be saved." };
            }
        }
        context.SaveChanges();

        // Save hall information
        foreach (ApartmentChoiceViewModel choice in apartmentChoices)
        {
            var newHallChoice = new Housing_HallChoices { HousingAppID = applicationID, HallName = choice.HallName, Ranking = choice.HallRank };
            var apartmentChoiceResult = context.Housing_HallChoices.Add(newHallChoice);
            if (apartmentChoiceResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The apartment preference could not be saved." };
            }
        }

        context.SaveChanges();

        return applicationID;
    }

    /// <summary>
    /// Edit an existings apartment application
    /// - first, it gets the EditorUsername from the database for the given application ID and makes sure that the student username of the current user matches that stored username
    /// - second, it gets an array of the applicants that are already stored in the database for the given application ID
    /// - third, it inserts each applicant that is in the 'newApplicantIDs' array but was not yet in the database
    /// - fourth, it removes each applicant that was stored in the database but was not in the 'newApplicantIDs' array
    ///
    /// </summary>
    /// <param name="username"> The student username of the user who is attempting to save the apartment application (retrieved via authentication token) </param>
    /// <param name="sess_cde"> The current session code </param>
    /// <param name="applicationID"> The application ID number of the application to be edited </param>
    /// <param name="newEditorUsername"> The student username of the student who is declared to be the editor of this application (retrieved from the JSON from the front end) </param>
    /// <param name="newApartmentApplicants"> Array of JSON objects providing apartment applicants </param>
    /// <param name="newApartmentChoices"> Array of JSON objects providing apartment hall choices </param>
    /// <returns>Returns the application ID number if all the queries succeeded</returns>
    public int EditApplication(string username, string sess_cde, int applicationID, string newEditorUsername, List<ApartmentApplicantViewModel> newApartmentApplicants, List<ApartmentChoiceViewModel> newApartmentChoices)
    {
        var editorUsername = context.Housing_Applications.Find(applicationID)?.EditorUsername;
        if (string.IsNullOrEmpty(editorUsername))
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
        }

        // Only perform the update if the username of the current user matched the 'EditorUsername' stored in the database for the requested application
        // This should already be caught by the StateYourBusiness, but I will leave this check here just in case
        if (username.ToLower() != editorUsername.ToLower())
        {
            throw new UnauthorizedAccessException("The current user does not match the stored editor of this application");
        }

        // Update applicant information
        // Get the IDs of the applicants that are already stored in the database for this application
        var existingApplicantResult = context.Housing_Applicants.Where(a => a.HousingAppID == applicationID);
        if (existingApplicantResult == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The applicants could not be found." };
        }

        // List of applicants IDs that are in the array recieved from the frontend but not yet in the database
        List<ApartmentApplicantViewModel> applicantsToAdd = new List<ApartmentApplicantViewModel>(newApartmentApplicants);

        // List of applicants IDs that are in both the array recieved from the frontend and the database
        List<ApartmentApplicantViewModel> applicantsToUpdate = new List<ApartmentApplicantViewModel>();

        // List of applicants IDs that are in the database but not in the array recieved from the frontend
        List<ApartmentApplicantViewModel> applicantsToRemove = new List<ApartmentApplicantViewModel>();

        // Check whether any applicants were found matching the given application ID number
        if (existingApplicantResult.Any())
        {
            foreach (Housing_Applicants existingApplicant in existingApplicantResult)
            {
                ApartmentApplicantViewModel newMatchingApplicant = null;
                newMatchingApplicant = newApartmentApplicants.FirstOrDefault(x => x.Username.ToLower() == existingApplicant.Username.ToLower());
                if (newMatchingApplicant != null)
                {
                    // If the applicant is in both the new applicant list and the existing applicant list, then we do NOT need to add it to the database
                    applicantsToAdd.Remove(newMatchingApplicant);
                    if (newMatchingApplicant.OffCampusProgram != existingApplicant.AprtProgram)
                    {
                        // If the applicant is in both the new and existing applicant lists but has different OffCampusProgram values, then we need to update that in the database
                        applicantsToUpdate.Add(newMatchingApplicant);
                    }
                }
                else
                {
                    ApartmentApplicantViewModel nonMatchingApplicant = new ApartmentApplicantViewModel
                    {
                        ApplicationID = existingApplicant.HousingAppID,
                        Username = existingApplicant.Username, // Code for after we remade the AA_Applicants table
                    };
                    // If the applicant is in the existing list but not in the new list of applicants, then we need to remove it from the database
                    applicantsToRemove.Add(nonMatchingApplicant);
                }
            }
        }

        // Insert new applicants that are not yet in the database
        foreach (ApartmentApplicantViewModel applicant in applicantsToAdd)
        {
            var applicantResult = context.Housing_Applicants.Add(new Housing_Applicants
            {
                HousingAppID = applicationID,
                Username = applicant.Username,
                SESS_CDE = sess_cde,
                AprtProgram = applicant.OffCampusProgram ?? ""
            });
            if (applicantResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = $"Applicant {applicant.Username} could not be inserted." };
            }
        }

        // Update the info of applicants from the frontend that are already in the database
        foreach (ApartmentApplicantViewModel applicant in applicantsToUpdate)
        {
            var applicantResult = context.Housing_Applicants.Find(applicationID, applicant.Username);
            if (applicantResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = $"Applicant {applicant.Username} could not be updated." };
            }
            else
            {
                applicantResult.AprtProgram = applicant.OffCampusProgram;
            }
        }

        // Remove applicants from the database that were remove from the frontend
        foreach (ApartmentApplicantViewModel applicant in applicantsToRemove)
        {
            var applicantResult = context.Housing_Applicants.Remove(new Housing_Applicants { HousingAppID = applicationID, Username = applicant.Username });
            if (applicantResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = $"Applicant {applicant.Username} could not be removed." };
            }
        }

        //--------
        // Update hall information


        // Get the apartment preferences that are already stored in the database for this application
        var existingApartmentChoiceResult = context.Housing_HallChoices.Where(c => c.HousingAppID == applicationID);
        if (existingApartmentChoiceResult == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The hall information could not be found." };
        }

        // List of apartment choices that are in the array recieved from the frontend but not yet in the database
        List<ApartmentChoiceViewModel> apartmentChoicesToAdd = new List<ApartmentChoiceViewModel>(newApartmentChoices);

        // List of apartment choices that are in both the array recieved from the frontend and the database
        List<ApartmentChoiceViewModel> apartmentChoicesToUpdate = new List<ApartmentChoiceViewModel>();

        // List of apartment choices that are in the database but not in the array recieved from the frontend
        List<ApartmentChoiceViewModel> apartmentChoicesToRemove = new List<ApartmentChoiceViewModel>();

        // Check whether any apartment choices were found matching the given application ID number
        if (existingApartmentChoiceResult.Any())
        {
            foreach (var existingApartmentChoice in existingApartmentChoiceResult)
            {
                ApartmentChoiceViewModel newMatchingApartmentChoice = null;
                newMatchingApartmentChoice = newApartmentChoices.FirstOrDefault(x => x.HallName == existingApartmentChoice.HallName);
                if (newMatchingApartmentChoice != null)
                {
                    // If the apartment is in both the new apartment list and the existing apartment list, then we do NOT need to add it to the database
                    apartmentChoicesToAdd.Remove(newMatchingApartmentChoice);
                    if (newMatchingApartmentChoice.HallRank != existingApartmentChoice.Ranking)
                    {
                        // If the apartment is in both the new and existing apartment lists but has different ranking values, then we need to update that in the database
                        apartmentChoicesToUpdate.Add(newMatchingApartmentChoice);
                    }
                }
                else
                {
                    ApartmentChoiceViewModel nonMatchingApartmentChoice = new ApartmentChoiceViewModel
                    {
                        ApplicationID = existingApartmentChoice.HousingAppID,
                        HallRank = existingApartmentChoice.Ranking,
                        HallName = existingApartmentChoice.HallName,
                    };
                    // If the apartment is in the existing list but not in the new list of apartments, then we need to remove it from the database
                    apartmentChoicesToRemove.Add(nonMatchingApartmentChoice);
                }
            }
        }

        // Insert new apartment choices that are not yet in the database
        foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToAdd)
        {
            var apartmentChoiceResult = context.Housing_HallChoices.Add(new Housing_HallChoices { HousingAppID = applicationID, HallName = apartmentChoice.HallName, Ranking = apartmentChoice.HallRank });
            if (apartmentChoiceResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = $"Apartment choice with ID {applicationID} and hall name {apartmentChoice.HallName} could not be inserted." };
            }
        }

        // Update the info of apartment choices from the frontend that are already in the database
        foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToUpdate)
        {
            var apartmentChoiceResult = context.Housing_HallChoices.FirstOrDefault(c => c.HousingAppID == applicationID && c.HallName == apartmentChoice.HallName);
            if (apartmentChoiceResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be updated." };
            }
            else
            {
                apartmentChoiceResult.Ranking = apartmentChoice.HallRank;
            }
        }

        // Remove apartment choices from the database that were removed from the frontend
        foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToRemove)
        {
            var apartmentChoiceResult = context.Housing_HallChoices.FirstOrDefault(c => c.HousingAppID == applicationID && c.HallName == apartmentChoice.HallName);
            if (apartmentChoiceResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be removed." };
            }
            else
            {
                context.Housing_HallChoices.Remove(apartmentChoiceResult);
            }
        }

        // Update the date modified (and application editor if necessary)
        var result = context.Housing_Applications.Find(applicationID);
        if (result == null)
        {
            throw new ResourceCreationException() { ExceptionMessage = "The application could not be updated." };
        }
        else
        {
            result.DateModified = DateTime.Now;
            if (newEditorUsername.ToLower() != editorUsername.ToLower())
            {
                result.EditorUsername = newEditorUsername;
            }
        }

        context.SaveChanges();

        return applicationID;
    }

    /// <summary>
    /// Changes the student user who has permission to edit the given application
    ///
    /// </summary>
    /// <returns>Whether or not all the queries succeeded</returns>
    public bool ChangeApplicationEditor(string username, int applicationID, string newEditorUsername)
    {
        var application = context.Housing_Applications.Find(applicationID);
        if (application == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
        }

        // Only perform the update if the username of the current user matched the 'EditorUsername' username stored in the database for the requested application
        if (username.ToLower() != application.EditorUsername.ToLower())
        {
            // Throw an error if the current user does not match this application's editor stored in the database
            throw new UnauthorizedAccessException("The current user does not match the stored editor of this application");
        }

        application.EditorUsername = newEditorUsername;

        context.SaveChanges();

        return true;
    }

    /// <param name="applicationID">application ID number of the apartment application</param>
    /// <param name="isAdmin">boolean indicating whether the current user is an admin, permits access to restricted information such as birth date</param>
    /// <returns>Apartment Application formatted for display in the UI</returns>
    public ApartmentApplicationViewModel GetApartmentApplication(int applicationID, bool isAdmin = false)
    {
        var applicationDBModel = context.Housing_Applications.Find(applicationID);
        if (applicationDBModel == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
        }

        // Assign the values from the database to the custom view model for the frontend
        ApartmentApplicationViewModel application = applicationDBModel; //implicit conversion

        var editorProfile = (StudentProfileViewModel?)context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == application.EditorUsername.ToLower());
        if (editorProfile == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The student information about the editor of this application could not be found." };
        }
        application.EditorProfile = editorProfile;

        // Get the applicants for this application
        application.Applicants = context.Housing_Applicants
            .Where(a => a.HousingAppID == applicationID)
            .OrderBy(a => a.Username)
            .Select<Housing_Applicants, ApartmentApplicantViewModel>(a => a)
            .AsEnumerable()
            .Select(a =>
            {
                var profile = context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == a.Username.ToLower());
                if (profile is not null)
                {
                    a.Profile = (StudentProfileViewModel)profile!;
                }
                return a;
            })
            .Where(a => a.Profile != null) // Exclude applicants without profile
            .ToList();


        if (isAdmin)
        {
            // Only add the birthdate, probabtion, and points if the user is authorized to view that information
            application.Applicants = application.Applicants
            .Select(applicant =>
            {
                applicant.BirthDate = context.ACCOUNT.FirstOrDefault(x => x.AD_Username.ToLower() == applicant.Username.ToLower())?.Birth_Date;

                // The probation data is already in the database, we just need to write a stored procedure to get it
                // applicantModel.Probation = ... // TBD

                // Calculate application points
                int points = 0;

                if (!string.IsNullOrEmpty(applicant.Class))
                {
                    points += int.Parse(applicant.Class);
                }

                if (applicant.Age >= 23)
                {
                    points += 1;
                }

                if (!string.IsNullOrEmpty(applicant.OffCampusProgram))
                {
                    points += 1;
                }

                if (applicant.Probation)
                {
                    points -= 3;
                }

                applicant.Points = Math.Max(0, points); ; // Set the resulting points to zero if the sum gave a value less than zero


                return applicant;
            }).ToList();
        }


        // Get the apartment choices for this application
        application.ApartmentChoices = context.Housing_HallChoices
            .Where(c => c.HousingAppID == applicationID)
            .OrderBy(c => c.Ranking)
            .ThenBy(c => c.HallName)
            .Select<Housing_HallChoices, ApartmentChoiceViewModel>(c => c)
            .ToList();

        return application;
    }

    /// <returns>Array of ApartmentApplicationViewModels</returns>
    public ApartmentApplicationViewModel[] GetAllApartmentApplication()
    {
        var applications = context.Housing_Applications.Include(a => a.Housing_Applicants).AsEnumerable();
        var currentSession = Helpers.GetCurrentSession(context);

        // TO DO: Refactor Housing App so that the application itself is connected
        //    to a session rather than the applicants!
        applications = applications.Where(a => a.Housing_Applicants.Any(
            apl => apl.SESS_CDE == currentSession
        ));

        return applications.Select(a => GetApartmentApplication(a.HousingAppID, true)).ToArray();
    }

    /// <summary>
    /// "Submit" an application by changing its DateSubmitted value to the date the submit button is succesfully clicked
    /// </summary>
    /// <param name="applicationID"> The application ID number of the application to be submitted </param>
    /// <returns>Returns whether the query succeeded</returns>
    public bool ChangeApplicationDateSubmitted(int applicationID)
    {
        var application = context.Housing_Applications.Find(applicationID);

        if (application == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found" };
        }

        application.DateSubmitted = DateTime.Now;
        context.SaveChanges();
        return true;
    }

    /// <summary>
    /// update roommate imformation
    /// </summary>
    /// <param name="username"> The username for the user who complete the aplication form </param>
    /// <param name="application_id"> The ID of this application </param>
    /// <param name="emailList"> A list of applicants' emails </param>
    public async Task UpdateRoommateAsync(string username, string application_id, string[] emailList)
    {
        var account = accountService.GetAccountByUsername(username);
        string gender = context.Student.FirstOrDefault(a => a.ID == account.GordonID).Gender;

        var maxYear = context.Student
            .Where(s => emailList.Contains(s.Email))
            .Select(s => s.Class).Max();
        await context.Year.AddAsync(new Year
        {
            ApplicationID = application_id,
            Year1 = maxYear
        });

        foreach (string e in emailList)
        {
            if (e != "")
            {
                var existActiveApplicant = context.Applicant.FirstOrDefault(x => (x.Applicant1 == e) && (x.Active == 1));
                if (existActiveApplicant != null)
                {
                    existActiveApplicant.Active = 0;
                }
                var student = context.Student.FirstOrDefault(x => x.Email == e);
                if (student == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The applicant cannot be found." };
                }
                if (gender != student.Gender)
                {
                    throw new BadInputException() { ExceptionMessage = "The applicants are not of the same gender." };
                }
                gender = student.Gender;
                var newApplicant = new Applicant
                {
                    ApplicationID = application_id,
                    Applicant1 = e,
                    Active = 1
                }; ;
                await context.Applicant.AddAsync(newApplicant);
            }
        }
        context.SaveChanges();
    }

    /// <summary>
    /// update the information of preferred halls
    /// </summary>
    /// <param name="username"> The username for the user who complete the aplication form </param>
    /// <param name="application_id"> The ID of this application </param>
    /// <param name="hallList"> A list of the preferred halls </param>
    public void UpdatePreferredHall(string username, string application_id, string[] hallList)
    {
        var hallPreferences = hallList.Select((hall, index) => new PreferredHall { ApplicationID = application_id, Rank = index + 1, HallName = hall });
        context.PreferredHall.AddRange(hallPreferences);
        context.SaveChanges();
    }

    /// <summary>
    /// update the information of preferred halls
    /// </summary>
    /// <param name="username"> The username for the user who complete the aplication form </param>
    /// <param name="application_id"> The ID of this application </param>
    /// <param name="preferenceList"> A list of the preference </param>
    public void UpdatePreference(string username, string application_id, string[] preferenceList)
    {
        var preferences = preferenceList.Select((preference) => new Preference { ApplicationID = application_id, Preference1 = preference });
        context.Preference.AddRange(preferences);
        context.SaveChanges();
    }

    /// <summary>
    /// update the information of due date
    /// </summary>
    /// <param name="dueDate"> The due date of the application </param>
    public async Task UpdateDueDateAsync(string dueDate)
    {
        // This code here is not working correctly. It cannot save the changes.
        var myDueDate = context.Config.FirstOrDefault(d => d.Key == "housing_lottery_due_date");
        myDueDate.Value = dueDate;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove the particular user from the current application by setting her/his active flag to 0
    /// </summary>
    /// <returns> Whether or not this was successful </returns>
    public bool RemoveUser(string username)
    {
        var email = context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username)?.email;
        var existActiveApplicant = context.Applicant.FirstOrDefault(x => (x.Applicant1 == email) && (x.Active == 1));
        if (existActiveApplicant == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The applicant could not be found" };
        }
        existActiveApplicant.Active = 0;
        context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Gets an array of preferences
    /// </summary>
    /// <returns> An array of preferences </returns>
    public IEnumerable<HousingPreferenceViewModel> GetAllPreferences()
    {
        var activeApplicants = context.Applicant.Where(a => a.Active == 1);

        var result = context.Preference
        .Join(activeApplicants, p => p.ApplicationID, a => a.ApplicationID, (p, a) => new HousingPreferenceViewModel
        {
            ApplicationID = p.ApplicationID,
            Preference1 = p.Preference1,
        }).Distinct();
        if (result == null || !result.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The preferences could not be found." };
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets an array of preferences of this user
    /// </summary>
    /// <returns> An array of preferences </returns>
    public IEnumerable<HousingPreferenceViewModel> GetUserPreference(string username)
    {
        var email = context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username)?.email;
        var applicationID = context.Applicant.FirstOrDefault(a => (a.Applicant1 == email) && (a.Active == 1))?.ApplicationID;
        var preference = context.Preference.Where(a => a.ApplicationID == applicationID).Select(ph => (HousingPreferenceViewModel)ph);
        return preference.ToArray();
    }

    /// <summary>
    ///Gets an array of preferred halls
    /// </summary>
    /// <returns> AN array of preferred halls </returns>
    public IEnumerable<HousingPreferredHallViewModel> GetAllPreferredHalls()
    {
        var activeApplicants = context.Applicant.Where(a => a.Active == 1);

        var result = context.PreferredHall
        .Join(activeApplicants, ph => ph.ApplicationID, a => a.ApplicationID, (ph, a) => new HousingPreferredHallViewModel
        {
            ApplicationID = ph.ApplicationID,
            Rank = ph.Rank,
            HallName = ph.HallName,
        }).Distinct();
        if (result == null || !result.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The preferred halls could not be found." };
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets an array of preferred hall of the user
    /// </summary>
    /// <returns> An array of preferences </returns>
    public IEnumerable<HousingPreferredHallViewModel> GetUserPreferredHall(string username)
    {
        var email = context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username)?.email;
        var applicationID = context.Applicant.FirstOrDefault(a => (a.Applicant1 == email) && (a.Active == 1))?.ApplicationID;
        var preferredHall = context.PreferredHall.Where(a => a.ApplicationID == applicationID).Select(ph => (HousingPreferredHallViewModel)ph);
        return preferredHall.ToArray();
    }

    /// <summary>
    ///Gets an array of applicants
    /// </summary>
    /// <returns> AN array of applicants </returns>
    public IEnumerable<HousingApplicantViewModel>  GetAllApplicants()
    {
        var result = context.Applicant.Where(a => a.Active == 1).Select(a => (HousingApplicantViewModel) a);
        if (result == null || !result.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The applicants could not be found." };
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets an array of preferences
    /// </summary>
    /// <returns> An array of preferences </returns>
    public IEnumerable<HousingApplicantViewModel> GetUserRoommate(string username)
    {
        var email = context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username)?.email;
        var applicationID = context.Applicant.FirstOrDefault(a => (a.Applicant1 == email) && (a.Active == 1))?.ApplicationID;
        var applicant = context.Applicant.Where(a => a.ApplicationID == applicationID).Select(a => (HousingApplicantViewModel)a);
        return applicant.ToArray();
    }

    /// <summary>
    ///Gets an array of school years
    /// </summary>
    /// <returns> AN array of school years </returns>
    public Year[] GetAllSchoolYear()
    {

        var result = context.Year;
        if (result == null || !result.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The school years could not be found." };
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets the due date of housing application
    /// </summary>
    /// <returns> The due date of housing application </returns>
    public string GetDueDate()
    {
        var result = context.DueDate.FirstOrDefault().DueDate1;
        return result;
    }
}