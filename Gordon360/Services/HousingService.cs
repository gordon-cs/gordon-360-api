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

namespace Gordon360.Services
{
    public class HousingService : IHousingService
    {
        private CCTContext _context;

        public HousingService(CCTContext context)
        {
            _context = context;
        }

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
                var result = _context.Housing_Applications.Remove(new Housing_Applications { HousingAppID = applicationID });
                _context.SaveChanges();
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

            var hallsResult = _context.Housing_Halls.Where(h => h.Type == "Apartment").Select(h => h.Name);
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
            return _context.Housing_Applicants.Where(a => a.Username == username && a.SESS_CDE == sess_cde).Select(a => a.HousingAppID).FirstOrDefault();
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
            return _context.Housing_Applications.Where(a => a.HousingAppID == applicationID).Select(a => a.EditorUsername).FirstOrDefault();
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
            var newAppResult = _context.Housing_Applications.Add(new Housing_Applications { DateModified = DateTime.Now, EditorUsername = editorUsername });

            _context.SaveChanges();

            if (newAppResult?.Entity == null || newAppResult?.Entity?.HousingAppID == 0)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The application could not be saved." };
            }

            // Retrieve the application ID number of this new application
            int applicationID = newAppResult.Entity.HousingAppID;

            // Save applicant
            foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
            {
                var applicantResult = _context.Housing_Applicants.Add(new Housing_Applicants { HousingAppID = applicationID, Username = applicant.Username, AprtProgram = applicant.OffCampusProgram ?? "", AprtProgramCredit = false, SESS_CDE = sess_cde });
                if (applicantResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = $"Applicant {applicant.Username} could not be saved." };
                }
            }
            _context.SaveChanges();

            // Save hall information
            foreach (ApartmentChoiceViewModel choice in apartmentChoices)
            {
                var newHallChoice = new Housing_HallChoices { HousingAppID = applicationID, HallName = choice.HallName, Ranking = choice.HallRank };
                var apartmentChoiceResult = _context.Housing_HallChoices.Add(newHallChoice);
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The apartment preference could not be saved." };
                }
            }

            _context.SaveChanges();

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
            var editorUsername = _context.Housing_Applications.Find(applicationID)?.EditorUsername;
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
            var existingApplicantResult = _context.Housing_Applicants.Where(a => a.HousingAppID == applicationID);
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
                var applicantResult = _context.Housing_Applicants.Add(new Housing_Applicants
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
                var applicantResult = _context.Housing_Applicants.Find(applicationID, applicant.Username);
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
                var applicantResult = _context.Housing_Applicants.Remove(new Housing_Applicants { HousingAppID = applicationID, Username = applicant.Username });
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = $"Applicant {applicant.Username} could not be removed." };
                }
            }

            //--------
            // Update hall information


            // Get the apartment preferences that are already stored in the database for this application
            var existingApartmentChoiceResult = _context.Housing_HallChoices.Where(c => c.HousingAppID == applicationID);
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
                var apartmentChoiceResult = _context.Housing_HallChoices.Add(new Housing_HallChoices { HousingAppID = applicationID, HallName = apartmentChoice.HallName, Ranking = apartmentChoice.HallRank });
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = $"Apartment choice with ID {applicationID} and hall name {apartmentChoice.HallName} could not be inserted." };
                }
            }

            // Update the info of apartment choices from the frontend that are already in the database
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToUpdate)
            {
                var apartmentChoiceResult = _context.Housing_HallChoices.FirstOrDefault(c => c.HousingAppID == applicationID && c.HallName == apartmentChoice.HallName);
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
                var apartmentChoiceResult = _context.Housing_HallChoices.FirstOrDefault(c => c.HousingAppID == applicationID && c.HallName == apartmentChoice.HallName);
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be removed." };
                }
                else
                {
                    _context.Housing_HallChoices.Remove(apartmentChoiceResult);
                }
            }

            // Update the date modified (and application editor if necessary)
            var result = _context.Housing_Applications.Find(applicationID);
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

            _context.SaveChanges();

            return applicationID;
        }

        /// <summary>
        /// Changes the student user who has permission to edit the given application
        ///
        /// </summary>
        /// <returns>Whether or not all the queries succeeded</returns>
        public bool ChangeApplicationEditor(string username, int applicationID, string newEditorUsername)
        {
            var application = _context.Housing_Applications.Find(applicationID);
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

            _context.SaveChanges();

            return true;
        }

        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <param name="isAdmin">boolean indicating whether the current user is an admin, permits access to restricted information such as birth date</param>
        /// <returns>Apartment Application formatted for display in the UI</returns>
        public ApartmentApplicationViewModel GetApartmentApplication(int applicationID, bool isAdmin = false)
        {
            var applicationDBModel = _context.Housing_Applications.Find(applicationID);
            if (applicationDBModel == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }

            // Assign the values from the database to the custom view model for the frontend
            ApartmentApplicationViewModel application = applicationDBModel; //implicit conversion

            var editorProfile = (StudentProfileViewModel?)_context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == application.EditorUsername.ToLower());
            if (editorProfile == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student information about the editor of this application could not be found." };
            }
            application.EditorProfile = editorProfile;

            // Get the applicants for this application
            application.Applicants = _context.Housing_Applicants
                .Where(a => a.HousingAppID == applicationID)
                .OrderBy(a => a.Username)
                .Select<Housing_Applicants, ApartmentApplicantViewModel>(a => a)
                .AsEnumerable()
                .Select(a =>
                {
                    var profile = _context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == a.Username.ToLower());
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
                    applicant.BirthDate = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username.ToLower() == applicant.Username.ToLower())?.Birth_Date;

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
            application.ApartmentChoices = _context.Housing_HallChoices
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
            var enumerable = _context.Housing_Applications.AsEnumerable();
            var currentSession = Helpers.GetCurrentSession(_context);

            // TO DO: Refactor Housing App so that the application itself is connected
            //    to a session rather than the applicants!
            enumerable = enumerable.Where(a => _context.Housing_Applicants.Where(
                apl => apl.HousingAppID == a.HousingAppID
            ).Any(
                apl => apl.SESS_CDE == currentSession
            ));

            return enumerable.Select(a => GetApartmentApplication(a.HousingAppID, true)).ToArray();
        }

        /// <summary>
        /// "Submit" an application by changing its DateSubmitted value to the date the submit button is succesfully clicked
        /// </summary>
        /// <param name="applicationID"> The application ID number of the application to be submitted </param>
        /// <returns>Returns whether the query succeeded</returns>
        public bool ChangeApplicationDateSubmitted(int applicationID)
        {
            var application = _context.Housing_Applications.Find(applicationID);

            if (application == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found" };
            }

            application.DateSubmitted = DateTime.Now;
            _context.SaveChanges();
            return true;
        }
    }
}
