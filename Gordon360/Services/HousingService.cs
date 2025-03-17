using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Static.Methods;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace Gordon360.Services;

public class HousingService(CCTContext context) : IHousingService
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
    /// Creates a new hall assignment range if it does not overlap with any existing ranges
    /// </summary>
    /// <param name="model">The ViewModel that contains the hall ID and room range</param>
    /// <returns>The created Hall_Assignment_Ranges object</returns>
    public async Task<Hall_Assignment_Ranges> CreateRoomRangeAsync(HallAssignmentRangeViewModel model)
    {


        // Check if Room_End is greater than Room_Start
        if (model.Room_End <= model.Room_Start)
        {
            throw new BadInputException() { ExceptionMessage = "Room_End must be greater than Room_Start." };
        }
        // Check if there is any overlapping room ranges in the same hall
        var overlappingRange = await context.Hall_Assignment_Ranges
            .AnyAsync(r => r.Hall_ID == model.Hall_ID
                && ((r.Room_Start <= model.Room_Start && r.Room_End >= model.Room_Start) ||
                    (r.Room_Start <= model.Room_End && r.Room_End >= model.Room_End)));

        if (overlappingRange)
        {
            throw new InvalidOperationException("The room range overlaps with an existing range in this hall.");
        }

        // Create a new Hall_Assignment_Ranges object
        var newRange = new Hall_Assignment_Ranges
        {
            Hall_ID = model.Hall_ID,
            Room_Start = model.Room_Start,
            Room_End = model.Room_End,
            Assigned_RA = null
        };

        // Add to the context and save changes
        context.Hall_Assignment_Ranges.Add(newRange);
        await context.SaveChangesAsync();

        return newRange;
    }

    /// <summary>
    /// Deletes a Room Range
    /// </summary>
    /// <param name="rangeId">The ID of the room range to delete</param>
    /// <returns> Returns if completed</returns>
    public async Task<bool> DeleteRoomRangeAsync(int rangeId)
    {
        // Find the room range by ID
        var roomRange = await context.Hall_Assignment_Ranges.FindAsync(rangeId);


        if (roomRange == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "Room range not found." };
        }

        context.Hall_Assignment_Ranges.Remove(roomRange);

        await context.SaveChangesAsync();

        return true;
    }


    /// <summary>
    /// Assigns an RA to a room range if no RA is currently assigned
    /// </summary>
    /// <param name="rangeId">The ID of the room range</param>
    /// <param name="raId">The ID of the RA to assign</param>
    /// <returns>The created RA_Assigned_Ranges object</returns>
    public async Task<Hall_Assignment_Ranges> AssignRaToRoomRangeAsync(int rangeId, string raId)
    {
        // Check if a different RA is already assigned to the range
        var existingAssignment = await context.Hall_Assignment_Ranges
            .Where(r => r.Range_ID == rangeId).FirstOrDefaultAsync();

        if (existingAssignment.Assigned_RA != null)
        {
            throw new InvalidOperationException("This room range already has an RA assigned.");
        }

        // Create the new RA assignment
        existingAssignment.Assigned_RA = raId;

        await context.SaveChangesAsync();

        return existingAssignment;
    }

    /// <summary>
    /// Deletes an RA range assignment
    /// </summary>
    /// <param name="rangeId">The Room range of the assignment to delete</param>
    /// <returns> Returns if completed</returns>
    public async Task<bool> DeleteAssignmentAsync(int rangeId)
    {
        // Find the assignment by range id
        var Assigment = await context.Hall_Assignment_Ranges
                                    .FirstOrDefaultAsync(r => r.Range_ID == rangeId);

        if (Assigment == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "Assignment not found." };
        }

        Assigment.Assigned_RA = null;

        await context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retrieve the RD of the resident's hall based on their hall ID.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <returns>Returns the RD's details if found, otherwise null.</returns>
    public async Task<RD_StudentsViewModel> GetResidentRDAsync(string hallId)
    {
        // Get the full details of the RD for the specified hall
        var hallRD = await context.RD_Info
            .Where(rd => rd.BuildingCode == hallId)
            .Select(rd => new RD_StudentsViewModel
            {
                HallName = rd.HallName,
                BuildingCode = rd.BuildingCode,
                RD_Email = rd.RD_Email,
                RD_Id = rd.RDId,
                RD_Name = rd.RDName
            })
            .FirstOrDefaultAsync();

        if (hallRD == null)
        {
            throw new InvalidOperationException("No RD found for the specified hall.");
        }

        return hallRD;
    }

    /// <summary>
    /// Retrieves a distinct list of all RDs with their IDs and names.
    /// </summary>
    /// <returns>
    /// Returns a list of RD_StudentsViewModel objects containing RD IDs and names.
    /// </returns>
    public async Task<List<RD_StudentsViewModel>> GetRDsAsync()
    {
        var rdList = await context.RD_Info
            .Select(rd => new RD_StudentsViewModel
            {
                RD_Id = rd.RDId,
                RD_Name = rd.RDName
            })
            .Distinct()
            .ToListAsync();

        return rdList;
    }

    /// <summary>
    /// Creates an RD's on-call assignment.
    /// </summary>
    /// <param name="OnCall">The ID of the RD</param>
    /// <returns>The created RD on-call assignment</returns>
    public async Task<RdOnCallGetView> CreateRdOnCallAsync(RD_On_Call_Create OnCall)
    {
        if (OnCall.Start_Date > OnCall.End_Date)
        {
            throw new BadInputException() { ExceptionMessage = "Start date cannot be after end date." };
        }

        // Check for overlapping RD on-call records
        bool overlapExists = await context.RD_On_Call
        .AnyAsync(r => r.Start_Date <= OnCall.End_Date && r.End_Date >= OnCall.Start_Date);

        if (overlapExists)
        {
            throw new BadInputException() { ExceptionMessage = "An existing on-call record overlaps with the given dates." };
        }

        // Create a new RD on-call record
        var newOnCall = new RD_On_Call
        {
            RD_ID = OnCall.RD_ID,
            Start_Date = OnCall.Start_Date,
            End_Date = OnCall.End_Date,
            Created_Date = DateTime.Now
        };

        context.RD_On_Call.Add(newOnCall);
        await context.SaveChangesAsync();

        return new RdOnCallGetView
        {
            Record_ID = newOnCall.Record_ID,
            RD_ID = newOnCall.RD_ID,
            Start_Date = newOnCall.Start_Date,
            End_Date = newOnCall.End_Date,
            Created_Date = newOnCall.Created_Date
        };
    }

    /// <summary>
    /// Updates an existing RD on-call record by its record ID.
    /// </summary>
    /// <param name="recordId">The unique identifier of the RD on-call record to update.</param>
    /// <param name="updatedOnCall">The updated RD on-call details.</param>
    /// <returns>
    /// Returns an updated RdOnCallGetView object if successful.
    /// </returns>
    public async Task<RdOnCallGetView> UpdateRdOnCallAsync(int recordId, RD_On_Call_Create updatedOnCall)
    {
        var existingOnCall = await context.RD_On_Call
            .FirstOrDefaultAsync(r => r.Record_ID == recordId);

        if (existingOnCall == null)
        {
            return null; // RD entry not found
        }

        if (updatedOnCall.Start_Date > updatedOnCall.End_Date)
        {
            throw new BadInputException() { ExceptionMessage = "Start date cannot be after end date." };
        }

        // Check for overlapping RD on-call records
        bool overlapExists = await context.RD_On_Call
        .Where(r => r.Record_ID != recordId)
        .AnyAsync(r => r.Start_Date <= updatedOnCall.End_Date && r.End_Date >= updatedOnCall.Start_Date);

            if (overlapExists)
            {
                throw new BadInputException() { ExceptionMessage = "An existing on-call record overlaps with the given dates." };
            }

        bool isUpdated = false;

        // Update fields only if the new value is different from the existing value
        if (updatedOnCall.RD_ID != existingOnCall.RD_ID)
        {
            existingOnCall.RD_ID = updatedOnCall.RD_ID;
            isUpdated = true;
        }

        if (updatedOnCall.Start_Date != existingOnCall.Start_Date)
        {
            existingOnCall.Start_Date = updatedOnCall.Start_Date;
            isUpdated = true;
        }

        if (updatedOnCall.End_Date != existingOnCall.End_Date)
        {
            existingOnCall.End_Date = updatedOnCall.End_Date;
            isUpdated = true;
        }

        if (!isUpdated)
        {
            throw new BadInputException() { ExceptionMessage = "No changes detected." };
        }


        await context.SaveChangesAsync();

        return new RdOnCallGetView
        {
            Record_ID = existingOnCall.Record_ID,
            RD_ID = existingOnCall.RD_ID,
            Start_Date = existingOnCall.Start_Date,
            End_Date = existingOnCall.End_Date,
            Created_Date = existingOnCall.Created_Date,
        };
    }

    /// <summary>
    /// Deletes an RD on-call record by its record ID.
    /// </summary>
    /// <param name="recordId">The unique identifier of the RD on-call record to delete.</param>
    /// <returns>
    /// Returns true if the record was successfully deleted.
    /// </returns>
    public async Task<bool> DeleteRDOnCallById(int recordId)
    {
        var rdEntry = await context.RD_On_Call
            .FirstOrDefaultAsync(r => r.Record_ID == recordId);

        if (rdEntry == null)
        {
            return false;
        }

        context.RD_On_Call.Remove(rdEntry);
        await context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retrieves the RD on call
    /// </summary>
    /// <returns>info for the RD.</returns>
    public async Task<RD_StudentsViewModel> GetRDOnCall()
    {
        var rd = await context.RD_OnCall_Today
            .Select(r => new RD_StudentsViewModel
            {
                RD_Email = r.RD_Email,
                RD_Id = r.RDId,
                RD_Name = r.RDName,
                RD_Photo = r.RD_Photo
            })
            .FirstOrDefaultAsync();

        return rd;
    }

    /// <summary>
    /// Retrieves a list of active RD on-call records where the end date is today or in the future.
    /// </summary>
    /// <returns>
    /// Returns a list of active RD on-call records
    /// If no active records are found, an empty list is returned.
    /// </returns>
    public async Task<List<RdOnCallGetView>> GetActiveRDOnCallsAsync()
    {
        var today = DateTime.Now.Date;

        var activeRDs = await context.RD_On_Call
            .Where(r => r.End_Date >= today)
            .Select(r => new RdOnCallGetView
            {
                Record_ID = r.Record_ID,
                RD_ID = r.RD_ID,
                Start_Date = r.Start_Date,
                End_Date = r.End_Date,
                Created_Date = r.Created_Date,
            })
            .ToListAsync();

        return activeRDs;
    }

    /// <summary>
    /// Retrieves the RA assigned to a resident based on their room number and hall ID.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <param name="roomNumber">The resident's room number.</param>
    /// <returns>Returns the RA's details if found, otherwise throws an exception.</returns>
    public async Task<RA_StudentsViewModel> GetResidentRAAsync(string hallId, string roomNumber)
    {
        // Query the room range within the specified hall that contains the room number
        var roomRange = await context.Hall_Assignment_Ranges
            .FirstOrDefaultAsync(r => r.Hall_ID == hallId
                && r.Room_Start <= int.Parse(roomNumber)
                && r.Room_End >= int.Parse(roomNumber));

        if (roomRange == null)
        {
            throw new InvalidOperationException("No RA found for the provided room number in the specified hall.");
        }

        // Find the RA assigned to that room range
        var assignedRAID = await context.Hall_Assignment_Ranges
            .Where(ra => ra.Range_ID == roomRange.Range_ID)
            .Select(ra => ra.Assigned_RA)
            .FirstOrDefaultAsync();

        if (assignedRAID == null)
        {
            throw new InvalidOperationException("No RA assigned to this room range.");
        }

        // Get the full details of the RA assigned to the room range
        var assignedRA = await context.RA_Students
            .Where(ra => ra.ID == assignedRAID)
            .Select(ra => new RA_StudentsViewModel
            {
                FirstName = ra.FirstName,
                LastName = ra.LastName,
                Dorm = ra.Dorm,
                BLDG_Code = ra.BLDG_Code,
                RoomNumber = ra.RoomNumber,
                Email = ra.Email,
                PhoneNumber = ra.PhoneNumber,
                ID = ra.ID,
                PhotoURL = ra.PhotoURL
            })
            .FirstOrDefaultAsync();

        if (assignedRA == null)
        {
            throw new InvalidOperationException("RA details could not be retrieved.");
        }

        // Fetch and include the preferred contact method for the RA
        var preferredContact = await GetPreferredContactAsync(assignedRA.ID);

        assignedRA.PreferredContact = preferredContact.Contact;

        return assignedRA;
    }

    /// <summary>
    /// Retrieves all room ranges.
    /// </summary>
    /// <returns>A list of room ranges.</returns>
    public async Task<List<HallAssignmentRangeViewModel>> GetAllRoomRangesAsync()
    {
        var roomRanges = await context.Hall_Assignment_Ranges
            .Select(r => new HallAssignmentRangeViewModel
            {
                RangeID = r.Range_ID,
                Hall_ID = r.Hall_ID,
                Room_Start = r.Room_Start,
                Room_End = r.Room_End
            })
            .ToListAsync();

        return roomRanges;
    }

    /// <summary>
    /// Retrieves all rooms missing a range.
    /// </summary>
    /// <returns>A list of rooms.</returns>
    public async Task<List<MissedRoomsViewModel>> GetMissedRoomsAsync()
    {
        var rooms = await context.Unassigned_Rooms
            .Select(r => new MissedRoomsViewModel
            {
                Room_Name = r.Room_Name,
                Building_Code = r.Building_Code,
                Room_Number = r.Room_Number
            })
            .ToListAsync();

        return rooms;
    }

    /// <summary>
    /// Retrieves a list of all RAs.
    /// </summary>
    /// <returns>Returns a list of RA_StudentsViewModel containing information about each RA</returns>
    public async Task<List<RA_StudentsViewModel>> GetAllRAsAsync()
    {
        var RAs = await context.RA_Students
            .Select(ra => new RA_StudentsViewModel
            {
                FirstName = ra.FirstName,
                LastName = ra.LastName,
                Dorm = ra.Dorm,
                BLDG_Code = ra.BLDG_Code,
                RoomNumber = ra.RoomNumber,
                Email = ra.Email,
                PhoneNumber = ra.PhoneNumber,
                ID = ra.ID
            })
            .ToListAsync();

        return RAs;
    }

    /// <summary>
    /// Retrieves the list of all assignments.
    /// </summary>
    /// <returns>Returns a list of all assignments</returns>
    public async Task<List<RA_Assigned_RangesViewModel>> GetRangeAssignmentsAsync()
    {
        var Assignments = await context.RA_Assigned_Ranges_View
            .Select(assignment => new RA_Assigned_RangesViewModel
            {
                RA_ID = assignment.RA_ID,
                Fname = assignment.Fname,
                Lname = assignment.Lname,
                Hall_Name = assignment.Hall_Name,
                Room_Start = assignment.Room_Start,
                Room_End = assignment.Room_End,
                Range_ID = assignment.Range_ID,
                Hall_ID = assignment.Hall_ID
            })
            .ToListAsync();

        return Assignments;
    }

    /// <summary>
    /// Retrieves the list of range assignments for a given RA_ID.
    /// </summary>
    /// <param name="raId">The RA_ID of the assigned RA.</param>
    /// <returns>Returns a list of assigned ranges for the specified RA.</returns>
    public async Task<List<RA_Assigned_RangesViewModel>> GetRangeAssignmentsByRAIdAsync(string raId)
    {
        var assignments = await context.RA_Assigned_Ranges_View
            .Where(assignment => assignment.RA_ID == raId)
            .Select(assignment => new RA_Assigned_RangesViewModel
            {
                RA_ID = assignment.RA_ID,
                Fname = assignment.Fname,
                Lname = assignment.Lname,
                Hall_Name = assignment.Hall_Name,
                Room_Start = assignment.Room_Start,
                Room_End = assignment.Room_End,
                Range_ID = assignment.Range_ID,
                Hall_ID = assignment.Hall_ID
            })
            .ToListAsync();

        return assignments;
    }




    /// <summary>
    /// Sets or updates an RA's preferred contact method
    /// </summary>
    /// <param name="raId">The ID of the RA</param>
    /// <param name="preferredContactMethod">The contact method (e.g., "Phone", "Teams")</param>
    /// <returns>True if the contact method was successfully set</returns>
    public async Task<bool> SetPreferredContactMethodAsync(string raId, string preferredContactMethod)
    {
        // Check if the RA already has a contact preference in the CCT model
        var existingPreference = await context.RA_Pref_Contact
            .FirstOrDefaultAsync(cp => cp.Ra_ID == raId);

        if (existingPreference != null)
        {
            // Update the existing preference
            existingPreference.Pref_contact = preferredContactMethod;
            context.RA_Pref_Contact.Update(existingPreference);
        }
        else
        {
            // Create a new preference using the CCT entity
            context.RA_Pref_Contact.Add(new RA_Pref_Contact
            {
                Ra_ID = raId,
                Pref_contact = preferredContactMethod
            });
        }

        // Save changes to the database
        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves the preferred contact information for an RA based on their contact preference.
    /// If the RA has a contact preference set, it will return either their phone number or a Microsoft Teams link 
    /// with their email embedded. If no preference exists, the method defaults to returning the RA's phone number.
    /// </summary>
    /// <param name="raId">The ID of the RA whose contact information is being requested.</param>
    /// <returns>A string containing the preferred contact information (phone number or Teams link) or a default 
    /// phone number if no preference is set.</returns>
    public async Task<RA_ContactPreference> GetPreferredContactAsync(string raId)
    {
        // Check if there is a preferred contact method for the given RA
        var contactPreference = await context.RA_Pref_Contact
            .FirstOrDefaultAsync(cp => cp.Ra_ID == raId);

        //find ra by id
        var ra = await context.RA_Students
                    .FirstOrDefaultAsync(r => r.ID == raId);

        // default contact to be phone
        var Contact = new RA_ContactPreference
        {
            Ra_ID = raId,
            PreferredContactMethod = "phone",
            Contact = ra?.PhoneNumber ?? "Phone number not found"
        };


        if (contactPreference != null)
        {
            // Determine the preferred method and get corresponding contact info
            if (contactPreference.Pref_contact == "phone")
            {
                    return Contact;
            }
            else if (contactPreference.Pref_contact == "teams")
            {
                // Fetch RA's email from the RA_Students table

                if (ra?.Email != null)
                {
                    // Generate Teams link using the email
                    Contact = new RA_ContactPreference
                    {
                        Ra_ID = raId,
                        PreferredContactMethod = "teams",
                        Contact = $"https://teams.microsoft.com/l/chat/0/0?users={ra.Email}"
                    };

                    return Contact; //unable to generate teams link, default to phone
                }
            }
        }

            // If no preference exists, return the phone number by default
            return Contact;
    }


    /// <summary>
    /// Gets the on-call RA's ID for specified hall.
    /// </summary>
    /// <param name="Hall_ID">The ID of the hall</param>
    /// <returns>The ID of the on-call RA, or null if no RA is currently on call</returns>
    public async Task<RA_On_Call_GetViewModel> GetOnCallRAAsync(string Hall_ID)
    {
        var onCallRA = await context.Current_On_Call 
            .Where(ra => ra.Hall_ID == Hall_ID)  // Filter by Hall_ID and only active check-ins
            .Select(ra => new RA_On_Call_GetViewModel
            {
                Hall_ID = ra.Hall_ID,
                Hall_Name = ra.Hall_Name,
                RoomNumber = ra.RoomNumber,
                RA_Name = ra.RA_Name,
                PreferredContact = ra.PreferredContact,
                Check_in_time = ra.Check_in_time,
                RD_Email = ra.RD_Email,
                RD_Name = ra.RD_Name,
                RA_UserName = ra.RA_UserName,
                RD_UserName = ra.RD_UserName,
                RA_Photo = ra.RA_Photo
            })
            .FirstOrDefaultAsync();

        return onCallRA;
    }

    /// <summary>
    /// Checks an RA in
    /// </summary>
    /// <param name="Ra_ID">Id of the ra checking in</param>
    ///<param name="Hall_IDs">The Hall(s) the RA is checking into</param>
    /// <returns>true if RA checked in successfully</returns>
    public async Task<bool> RA_CheckinAsync(string[] Hall_IDs, string Ra_ID)
    {
        foreach (string hallId in Hall_IDs)
        {
            // Check if there is an existing RA checked into this hall without an end time
            var existingRA = await context.RA_On_Call
                .Where(r => r.Hall_ID == hallId && r.Check_out_time == null)
                .FirstOrDefaultAsync();

            // If an existing RA is found, set their Check_out_time to the current time
            if (existingRA != null)
            {
                existingRA.Check_out_time = DateTime.Now;
                context.RA_On_Call.Update(existingRA);
            }

            // Add the new RA check-in record with no check-out time
            var newCheckin = new RA_On_Call
            {
                Ra_ID = Ra_ID,
                Hall_ID = hallId,
                Check_in_time = DateTime.Now,
                Check_out_time = null // RA has an active checkin
            };
            context.RA_On_Call.Add(newCheckin);
        }

        await context.SaveChangesAsync();
        return true;
    }

        /// <summary>
        /// Gets the on-call RAs for all halls.
        /// </summary>
        /// <returns>The RAs on call</returns>
        public async Task<List<RA_On_Call_GetViewModel>> GetOnCallRAAllHallsAsync()
        {
            var onCallRAs = await context.Current_On_Call
                .Select(oncall => new RA_On_Call_GetViewModel
                {
                    Hall_ID = oncall.Hall_ID,
                    Hall_Name = oncall.Hall_Name,
                    RA_Name = oncall.RA_Name,
                    PreferredContact = oncall.PreferredContact,
                    Check_in_time = oncall.Check_in_time,
                    RD_Email = oncall.RD_Email,
                    RA_UserName = oncall.RA_UserName,
                    RD_UserName = oncall.RD_UserName,
                    RD_Name = oncall.RD_Name,
                    RA_Photo = oncall.RA_Photo
                })
                .ToListAsync();
        
            return onCallRAs;
        }

    /// <summary>
    /// Gets the on-call RA's current halls
    /// </summary>
    /// <param name="userName">The username of the ra</param>
    /// <returns>The RA's current halls</returns>
    public async Task<List<string>> GetOnCallRAHallsAsync(string userName)
    {
        return await context.Current_On_Call
            .Where(oncall => oncall.RA_UserName == userName)
            .Select(oncall => oncall.Hall_ID)
            .ToListAsync();
    }


    /// <summary>
    /// Checks if an RA is currently on call.
    /// </summary>
    /// <param name="raId">The ID of the RA</param>
    /// <returns>True if the RA is on call, false otherwise</returns>
    public async Task<bool> IsRAOnCallAsync(string raId)
    {
        // Check if the RA is currently on call
        var isOnCall = await context.RA_On_Call
            .AnyAsync(ra => ra.Ra_ID == raId && ra.Check_out_time == null);

        return isOnCall;
    }
    /// <summary>
    /// Checks if a student is residential
    /// </summary>
    /// <param name="idNum">The ID of the student</param>
    /// <returns>True if the student is a resident</returns>
    public async Task<bool> IsStudentResidentialAsync(int idNum)
    {
        var isRes = await context.ResidentialStatus_View
                                .Where(s => s.Student_ID == idNum)
                                .Select(s => s.Is_Residential)
                                .FirstOrDefaultAsync();

        return isRes ?? false;
    }

    /// <summary>
    /// Creates a new task for the given hall
    /// </summary>
    /// <param name="task">The HallTaskViewModel object containing necessary info</param>
    /// <returns>The created task</returns>
    public async Task<HallTaskViewModel> CreateTaskAsync(HallTaskViewModel task)
    {
        var newTask = new Hall_Tasks
        {
            Name = task.Name,
            Description = task.Description,
            Hall_ID = task.HallID,
            Is_Recurring = task.IsRecurring,
            Frequency = task.Frequency,
            Interval = task.Interval,
            Start_Date = task.StartDate,
            End_Date = task.EndDate,
            Created_Date = DateTime.Now
        };

        // Check if task starts today and add to Task_Occurrence
        if (newTask.Start_Date.Date == DateTime.Now.Date)
        {
            var newOccurrence = new Hall_Task_Occurrence
            {
                Task_ID = newTask.Task_ID,
                OccurDate = DateTime.Now.Date,
                IsComplete = false,
                CompletedBy = null,
                CompletedDate = null
            };

            newTask.Hall_Task_Occurrence = [newOccurrence];
        }

        await context.Hall_Tasks.AddAsync(newTask);
        await context.SaveChangesAsync();
  

        return new HallTaskViewModel
        {
            TaskID = newTask.Task_ID,
            Name = newTask.Name,
            Description = newTask.Description,
            HallID = newTask.Hall_ID,
            IsRecurring = newTask.Is_Recurring,
            Frequency = newTask.Frequency,
            Interval = (int)newTask.Interval,
            StartDate = newTask.Start_Date,
            EndDate = newTask.End_Date,
            CreatedDate = newTask.Created_Date
        };
    }

    /// <summary>
    /// Updates the task by the given ID
    /// </summary>
    /// <param name="taskID">The HallTaskViewModel object containing necessary info</param>
    /// <param name="task">The HallTaskViewModel object containing necessary info</param>
    /// <returns>The created task</returns>
    public async Task<HallTaskViewModel> UpdateTaskAsync(int taskID, HallTaskViewModel task)
    {
        var existingTask = await context.Hall_Tasks.FindAsync(taskID);
        if (existingTask == null)
        {
            return null;
        }

        existingTask.Name = task.Name;
        existingTask.Description = task.Description;
        existingTask.Hall_ID = task.HallID;
        existingTask.Is_Recurring = task.IsRecurring;
        existingTask.Frequency = task.Frequency;
        existingTask.Interval = task.Interval;
        existingTask.Start_Date = task.StartDate;
        existingTask.End_Date = task.EndDate;

        context.Hall_Tasks.Update(existingTask);
        await context.SaveChangesAsync();

        return new HallTaskViewModel
        {
            TaskID = existingTask.Task_ID,
            Name = existingTask.Name,
            Description = existingTask.Description,
            HallID = existingTask.Hall_ID,
            IsRecurring = existingTask.Is_Recurring,
            Frequency = existingTask.Frequency,
            Interval = (int)existingTask.Interval,
            StartDate = existingTask.Start_Date,
            EndDate = existingTask.End_Date,
            CreatedDate = existingTask.Created_Date
        };
    }

    /// <summary>
    /// Disables a task
    /// </summary>
    /// <param name="taskID">The ID of the task to disable</param>
    /// <returns>True if disable</returns>
    public async Task<bool> DisableTaskAsync(int taskID)
    {
        var existingTask = await context.Hall_Tasks.FindAsync(taskID);
        if (existingTask == null)
        {
            return false;
        }
        existingTask.End_Date = DateTime.Now.AddDays(-1);
        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Marks a task completed
    /// </summary>
    /// <param name="taskID">the ID of the task to update</param>
    /// <param name="CompletedBy">The ID of the RA completing the task</param>
    /// <returns>True if completed</returns>
    public async Task<bool> CompleteTaskAsync(int taskID, string CompletedBy)
    {
        var existingTask = await context.Hall_Task_Occurrence.FindAsync(taskID);

        existingTask.CompletedDate = DateTime.Now;
        existingTask.CompletedBy = CompletedBy;
        existingTask.IsComplete = true;

        context.Hall_Task_Occurrence.Update(existingTask);
        await context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Marks a task not completed
    /// </summary>
    /// <param name="taskID">the ID of the task to update</param>
    /// <returns>True if marked not completed</returns>
    public async Task<bool> IncompleteTaskAsync(int taskID)
    {
        var existingTask = await context.Hall_Task_Occurrence.FindAsync(taskID);

        existingTask.CompletedDate = null;
        existingTask.CompletedBy = null;
        existingTask.IsComplete = false;

        context.Hall_Task_Occurrence.Update(existingTask);
        await context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Gets the list of active tasks
    /// </summary>
    /// <param name="hallId">the ID of the hall to get tasks for</param>
    /// <returns>The list of tasks</returns>
    public async Task<List<HallTaskViewModel>> GetActiveTasksForHallAsync(string hallId)
    {
        var tasks = await context.Hall_Tasks
            .Where(t => t.Hall_ID == hallId && (!t.End_Date.HasValue || t.End_Date.Value.Date >= DateTime.Now.Date))
            .Select(t => new HallTaskViewModel
            {
                TaskID = t.Task_ID,
                Name = t.Name,
                Description = t.Description,
                HallID = t.Hall_ID,
                IsRecurring = t.Is_Recurring,
                Frequency = t.Frequency,
                Interval = (int)t.Interval,
                StartDate = t.Start_Date,
                EndDate = t.End_Date,
                CreatedDate = t.Created_Date
            })
            .ToListAsync();

        return tasks;
    }

    /// <summary>
    /// Gets the list of daily tasks for a hall
    /// </summary>
    /// <param name="hallId">the ID of the hall to get tasks for</param>
    /// <returns>The list of daily tasks</returns>
    public async Task<List<DailyTaskViewModel>> GetTasksForHallAsync(string hallId)
    {
        var tasks = await context.CurrentTasks
            .Where(t => t.Hall_ID == hallId)
            .Select(t => new DailyTaskViewModel
            {
                TaskID = t.Task_ID,
                Name = t.Name,
                Description = t.Description,
                HallID = t.Hall_ID,
                CompletedDate = t.CompletedDate,
                CompletedBy = t.CompletedBy,
                OccurDate = t.OccurDate
            })
            .ToListAsync();

        return tasks;
    }

    /// <summary>
    /// Creates a new status event for an RA's schedule
    /// </summary>
    /// <param name="status">The RA_StatusEventsViewModel object containing necessary info</param>
    /// Swagger is showing the time inputs in a tick format but the below works and should be used
    /// {
    ///"statusID": 0,
    ///"raID": "RA123",
    ///"statusName": "On Duty",
    ///"isRecurring": true,
    ///"frequency": "Weekly",
    ///"interval": 1,
    ///"start_Time": "08:00:00",
    ///"end_Time": "09:00:00",
    ///"startDate": "2025-02-21",
    ///"endDate": "2025-02-21",
    ///"createdDate": "2025-02-21T07:45:00Z"
    ///}
/// <returns>The created status event</returns>
public async Task<RA_StatusEventsViewModel> CreateStatusEventAsync(RA_StatusEventsViewModel status)
    {

        bool hasOverlap = await context.RA_Status_Events.AnyAsync(existing =>
        existing.Ra_ID == status.RaID &&
        (
            (status.StartDate <= existing.End_Date && status.EndDate >= existing.Start_Date) &&
            (status.Start_Time < existing.End_Time && status.End_Time > existing.Start_Time)
        )
    );

        if (hasOverlap)
        {
            throw new InvalidOperationException("A conflicting status event already exists for this RA.");
        }

        var newStatus = new RA_Status_Events
        {
            Ra_ID = status.RaID,
            Status_Name = status.StatusName,
            Is_Recurring = status.IsRecurring,
            Frequency = status.Frequency,
            Interval = status.Interval,
            Start_Time = status.Start_Time,
            End_Time = status.End_Time,
            Start_Date = status.StartDate,
            End_Date = status.EndDate,
            Created_Date = DateTime.Now,
            Available = status.Available,

        };

        await context.RA_Status_Events.AddAsync(newStatus);
        await context.SaveChangesAsync();

        return new RA_StatusEventsViewModel
        {
            StatusID = newStatus.Status_ID,
            RaID = newStatus.Ra_ID,
            StatusName = newStatus.Status_Name,
            IsRecurring = newStatus.Is_Recurring,
            Frequency = newStatus.Frequency,
            Interval = (int)newStatus.Interval,
            Start_Time = newStatus.Start_Time,
            End_Time = newStatus.End_Time,
            StartDate = newStatus.Start_Date,
            EndDate = newStatus.End_Date,
            CreatedDate = newStatus.Created_Date,
            Available = newStatus.Available
        };
    }

    /// <summary>
    /// Deletes a status event for an RA's schedule
    /// </summary>
    /// <param name="statusID">The ID of the status event to delete</param>
    /// <returns>True if deleted</returns>
    public async Task<bool> DeleteStatusEventAsync(int statusID)
    {
        var existingStatus = await context.RA_Status_Events
            .FirstOrDefaultAsync(s => s.Status_ID == statusID);

        if (existingStatus == null)
        {
            return false;
        }

        existingStatus.End_Date = DateTime.Now.Date.AddDays(-1); // Mark status as ended
        existingStatus.End_Time = DateTime.Now.TimeOfDay;
        
        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates the RA status event by the given ID
    /// </summary>
    /// <param name="statusID">The ID of the status event to update</param>
    /// <param name="status">The RA_StatusEventsViewModel object containing necessary info</param>
    /// <returns>The updated status event</returns>
    public async Task<RA_StatusEventsViewModel> UpdateStatusEventAsync(int statusID, RA_StatusEventsViewModel status)
    {
        var existingStatus = await context.RA_Status_Events.FindAsync(statusID);
        if (existingStatus == null)
        {
            return null;
        }

        // Check for overlapping status events
        bool hasOverlap = await context.RA_Status_Events.AnyAsync(existing =>
            existing.Ra_ID == status.RaID &&
            existing.Status_ID != statusID && // Exclude the current status event
            (
                (status.StartDate <= existing.End_Date && status.EndDate >= existing.Start_Date) &&
                (status.Start_Time < existing.End_Time && status.End_Time > existing.Start_Time)
            )
        );

        if (hasOverlap)
        {
            throw new InvalidOperationException("A conflicting status event already exists for this RA.");
        }

        // Update the existing status event
        existingStatus.Status_Name = status.StatusName;
        existingStatus.Is_Recurring = status.IsRecurring;
        existingStatus.Frequency = status.Frequency;
        existingStatus.Interval = status.Interval;
        existingStatus.Start_Time = status.Start_Time;
        existingStatus.End_Time = status.End_Time;
        existingStatus.Start_Date = status.StartDate;
        existingStatus.End_Date = status.EndDate;
        existingStatus.Available = status.Available;

        context.RA_Status_Events.Update(existingStatus);
        await context.SaveChangesAsync();

        // Return the updated status event
        return new RA_StatusEventsViewModel
        {
            StatusID = existingStatus.Status_ID,
            RaID = existingStatus.Ra_ID,
            StatusName = existingStatus.Status_Name,
            IsRecurring = existingStatus.Is_Recurring,
            Frequency = existingStatus.Frequency,
            Interval = (int)existingStatus.Interval,
            Start_Time = existingStatus.Start_Time,
            End_Time = existingStatus.End_Time,
            StartDate = existingStatus.Start_Date,
            EndDate = existingStatus.End_Date,
            CreatedDate = existingStatus.Created_Date,
            Available = existingStatus.Available
        };
    }


    /// <summary>
    /// Gets the list of daily status events for an RA
    /// </summary>
    /// <param name="raID"> The ID of the RA</param>
    /// <returns>The list of daily status events</returns>
    public async Task<List<DailyStatusEventsViewModel>> GetStatusEventsForRAAsync(string raID)
    {
        var statusEvents = await context.Daily_RA_Events
            .Where(s => s.Ra_ID == raID)
            .Select(s => new DailyStatusEventsViewModel
            {
                StatusID = s.Status_ID,
                RaID = s.Ra_ID,
                StatusName = s.Status_Name,
                Start_Time = (TimeSpan)s.Start_Time,
                End_Time = (TimeSpan)s.End_Time,
                Start_Date = s.Start_Date,
                End_Date = s.End_Date,
                Available = s.Available
            })
            .ToListAsync();

        return statusEvents;
    }

    /// <summary>
    /// Gets the active statuses for a given RA.
    /// </summary>
    /// <param name="raId">The ID of the RA.</param>
    /// <returns>A list of active statuses or a 404 if none exist.</returns>
    public async Task<List<RA_StatusEventsViewModel>> GetActiveStatusesByRAIdAsync(string raId)
    {
        var activeStatuses = await context.RA_Status_Events
            .Where(status => status.Ra_ID == raId && status.End_Date >= DateTime.Now.Date)
            .Select(status => new RA_StatusEventsViewModel
            {
                StatusID = status.Status_ID,
                RaID = status.Ra_ID,
                StatusName = status.Status_Name,
                IsRecurring = status.Is_Recurring,
                Frequency = status.Frequency,
                Interval = status.Interval ?? 0,
                Start_Time = status.Start_Time,
                End_Time = status.End_Time,
                StartDate = status.Start_Date,
                EndDate = status.End_Date,
                CreatedDate = status.Created_Date,
                Available = status.Available
            })
            .ToListAsync();

        return activeStatuses;
    }






}
