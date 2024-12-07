﻿using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Services
{
    public class LostAndFoundService(CCTContext context) : ILostAndFoundService
    {
        /// <summary>
        /// Create a new missing item report, for the submitter in the details, or the authenticated user if that is null.
        /// </summary>
        /// <param name="reportDetails"></param>
        /// <param name="username"></param>
        /// <returns>Report ID - The ID of the report, generated by the database.</returns>
        /// <exception cref="ResourceCreationException"></exception>
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails, string username)
        {
            // By default, get the submitter's account from the report details passed by the frontend
            var account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == reportDetails.submitterUsername);

            // If that fails, use the account of the currently authenticated user
            if (account == null)
            {
                account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            }

            string idNum;

            if (account != null)
            {
                idNum = account.gordon_id;
            }
            else
            {
                throw new ResourceCreationException() { ExceptionMessage = "No account could be found for the user." };
            }

            // Create the new report using the supplied fields
            var newReportResults = context.MissingReports.Add(new MissingReports
            {
                submitterID = idNum,
                forGuest = reportDetails.forGuest,
                category = reportDetails.category,
                colors = string.Join(",", reportDetails.colors),
                brand = reportDetails.brand,
                description = reportDetails.description,
                locationLost = reportDetails.locationLost,
                stolen = reportDetails.stolen,
                stolenDesc = reportDetails.stolenDescription,
                dateLost = reportDetails.dateLost,
                dateCreated = reportDetails.dateCreated,
                status = reportDetails.status,
            });

            context.SaveChanges();

            if (newReportResults == null || newReportResults?.Entity?.ID == 0)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The report could not be saved." };
            }

            // /If this report was submitted for a guest user.
            if (reportDetails.forGuest)
            {
                // Add a guest user to the db associated with this report
                var newGuest = context.GuestUsers.Add(new GuestUsers
                {
                    missingID = newReportResults.Entity.ID,
                    firstName = reportDetails.firstName,
                    lastName = reportDetails.lastName,
                    phoneNumber = reportDetails.phone, 
                    emailAddress = reportDetails.email,
                });

                context.SaveChanges();

                if (newGuest.Entity == null || newReportResults?.Entity?.ID == 0)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The user associated with this record could not be saved." };
                }
            }

            // Retrieve the application ID number of this new application
            int reportID = newReportResults.Entity.ID;

            return reportID;
        }

        /// <summary>
        /// Create an ActionsTaken for a given missing item report.
        /// </summary>
        /// <param name="id">The id</param>
        /// <param name="ActionsTaken">The actions taken object to create</param>
        /// <returns>actionTakenID - The ID of the action taken, generated by the database.</returns>
        public int CreateActionTaken(int id, ActionsTakenViewModel ActionsTaken)
        {
            var account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == ActionsTaken.username);

            string idNum;

            if (account != null)
            {
                idNum = account.gordon_id;
            }
            else
            {
                throw new ResourceCreationException() { ExceptionMessage = "No account could be found for the user." };
            }

            var newActionTaken = context.ActionsTaken.Add(new ActionsTaken
            {
                missingID = id,
                action = ActionsTaken.action,
                actionNote = ActionsTaken.actionNote,
                actionDate = ActionsTaken.actionDate,
                submitterID = idNum,
                isPublic = ActionsTaken.isPublic,
            });

            context.SaveChanges();

            if (newActionTaken == null || newActionTaken?.Entity?.ID == 0)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The action could not be saved." };
            }

            int actionTakenID = newActionTaken.Entity.ID;

            return actionTakenID;
        }

        /// <summary>
        /// Update a report with given id, to the given report detail data.
        ///    NOTE: cannot update associated guest user, if this report is for guest.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reportDetails"></param>
        /// <returns></returns>
        /// <exception cref="ResourceNotFoundException"></exception>
        public async Task UpdateMissingItemReportAsync(int id, MissingItemReportViewModel reportDetails)
        {
            var original = await context.MissingReports.FindAsync(id);

            if (original != null)
            {
                original.category = reportDetails.category;
                original.colors = string.Join(",", reportDetails.colors);
                original.brand = reportDetails.brand;
                original.description = reportDetails.description;
                original.locationLost = reportDetails.locationLost;
                original.stolen = reportDetails.stolen;
                original.stolenDesc = reportDetails.stolenDescription;
                original.dateLost = reportDetails.dateLost;
                original.dateCreated = reportDetails.dateCreated;
                original.status = reportDetails.status;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Missing Item Report was not found" };
            }
        }

        /// <summary>
        /// Update the status of a report with given id, to the given status message
        ///     Status text must be in the set of allowed statuses, "Active", "Expired", "Deleted", "Found"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>None</returns>
        /// <exception cref="ResourceNotFoundException"></exception>
        public async Task UpdateReportStatusAsync(int id, string status)
        {
            var original = await context.MissingReports.FindAsync(id);

            if (original != null)
            {
                original.status = status;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Missing Item Report was not found" };
            }
        }

        /// <summary>
        /// Get the list of missing item reports for given user.
        /// </summary>
        /// <param name="username">The username to get reports for</param>
        /// <returns>an Enumerable of Missing Item Reports containing all missing item reports</returns>
        public IEnumerable<MissingItemReportViewModel> GetMissingItems(string username)
        {
            // Get the account of the given username.
            var account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

            string idNum;

            if (account != null)
            {
                idNum = account.gordon_id;
            }
            else
            {
                throw new ResourceCreationException() { ExceptionMessage = "No account could be found for the usename." };
            }
            // Fetch the missing item reports belonging to that user, and not submitted for someone else
            IEnumerable<MissingItemData> missingList = context.MissingItemData.Where(x => x.submitterID == idNum && x.forGuest == false);
            // Convert the report objects into viewmodel objects.
            IEnumerable<MissingItemReportViewModel> returnList = missingList.Select(x => (MissingItemReportViewModel)x);

            // Get the list of actions taken that are public, and add them to each missing item report.
            List<IEnumerable<ActionsTakenViewModel>> actionsTakenList = [];
            foreach (MissingItemReportViewModel item in returnList)
            {
                if (item.recordID != null)
                {
                    actionsTakenList.Add(GetActionsTaken((int)item.recordID, username, true));
                }
            }

            IEnumerator<IEnumerable<ActionsTakenViewModel>> actionsTakenEnumerator = actionsTakenList.GetEnumerator();
            // Mix-in actions taken lists for each report
            returnList = returnList.Select(x => { actionsTakenEnumerator.MoveNext(); x.adminActions = actionsTakenEnumerator.Current; return x; });

            return returnList;
        }

        /// <summary>
        /// Get all missing item reports
        /// </summary>
        /// <returns>An enumerable of Missing Item Reports, from the Missing Item Data view</returns>
        public IEnumerable<MissingItemReportViewModel> GetMissingItemsAll(string username)
        {
            IEnumerable<MissingItemReportViewModel> missingItems = context.MissingItemData.Select(x => (MissingItemReportViewModel)x).ToList();

            foreach (MissingItemReportViewModel item in missingItems)
            {
                if (item.recordID != null)
                {
                    item.adminActions = (GetActionsTaken((int)item.recordID, username));
                }
            }

            return missingItems;
        }

        /// <summary>
        /// Gets a Missing by id, only allowed if it belongs to the username, or the user is an admin
        /// </summary>
        /// <param name="missingItemID">The ID of the missing item</param>
        /// <param name="username">The username of the person requesting the data</param>
        /// <returns>A Missing Item Report object, or null if no item matches the id</returns>
        public MissingItemReportViewModel? GetMissingItem(int missingItemID, string username)
        {
            IEnumerable<Enums.AuthGroup> userGroups = Authorization.AuthUtils.GetGroups(username);
            bool isDev;
            bool isAdmin;

            // AD permission issues can, in rare cases, lead to errors enumerating userGroups:
            try
            {
                isDev = userGroups.Contains(Enums.AuthGroup.LostAndFoundDevelopers);
            }
            catch (NoMatchingPrincipalException e)
            {
                Log.Error("No Matching Principle Exception encountered when enumerating groups searching for LostAndFoundDevelopers, for USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isDev = false;
            }
            try
            {
                isAdmin = userGroups.Contains(Enums.AuthGroup.LostAndFoundAdmin);
            }
            catch (NoMatchingPrincipalException e)
            {
                Log.Error("No Matching Principle Exception encountered when enumerating groups searching for LostAndFoundAdmin, for USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isAdmin = false;
            }

            MissingItemReportViewModel report;
            // If user is admin or developer, simply get the report
            if (isAdmin || isDev)
            {
                var data = context.MissingItemData.FirstOrDefault(x => x.ID == missingItemID);
                if (data != null)
                {
                    report = (MissingItemReportViewModel)data;

                    // Get the list of all admin actions on this report, and add them to the report.
                    report.adminActions = GetActionsTaken(missingItemID, username);
                }
                else
                {
                    // If no such report exists
                    throw new ResourceNotFoundException();
                }
            }
            else
            {
                // Otherwise get the reportif it belongs to the requesting user
                var data = context.MissingItemData.FirstOrDefault(x => x.ID == missingItemID && x.submitterUsername == username);
                if (data != null)
                {
                    report = (MissingItemReportViewModel)data;

                    // Get the list of public admin actions on this report, and add them to the report.
                    report.adminActions = GetActionsTaken(missingItemID, username, true);
                }
                else
                {
                    // If no such report exists
                    throw new ResourceNotFoundException();
                }
            }
            
            return report;
        }

        /// <summary>
        /// Gets a list of Actions Taken by id, general users only allowed to get public actions on their own reports
        /// Attemps by a non-admin user to get actions for a report which doesn't belong to them will yield an UnauthorizedAccessException
        /// </summary>
        /// <param name="missingID">The ID of the associated missing item report</param>
        /// <param name="username">The username of the user requesting the information</param>
        /// <param name="getPublicOnly">Oonly get actions marked as public.  Default false.</param>
        /// <returns>An ActionsTaken[], or null if no item matches the id</returns>
        public IEnumerable<ActionsTakenViewModel> GetActionsTaken(int missingID, string username, bool getPublicOnly = false)
        {
            IEnumerable<Enums.AuthGroup> userGroups = Authorization.AuthUtils.GetGroups(username);
            bool isDev;
            bool isAdmin;

            // AD permission issues can, in rare cases, lead to errors enumerating userGroups:
            try
            {
                isDev = userGroups.Contains(Enums.AuthGroup.LostAndFoundDevelopers);
            }
            catch (NoMatchingPrincipalException e)
            {
                Log.Error("No Matching Principle Exception encountered when enumerating groups searching for LostAndFoundDevelopers, for USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isDev = false;
            }
            try
            {
                isAdmin = userGroups.Contains(Enums.AuthGroup.LostAndFoundAdmin);
            }
            catch (NoMatchingPrincipalException e)
            {
                Log.Error("No Matching Principle Exception encountered when enumerating groups searching for LostAndFoundAdmin, for USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isAdmin = false;
            }

            bool hasElevatedPermissions = (isAdmin || isDev);

            // Get all actions taken for the report
            IQueryable<ActionsTakenData> actionsList = context.ActionsTakenData.Where(x => x.missingID == missingID);

            // If an admin requests only public actions
            if (hasElevatedPermissions && getPublicOnly)
            {
                actionsList = actionsList.Where(x => x.isPublic);
            }
            // Otherwise if a general user requests actions for a report
            else if (!hasElevatedPermissions)
            {
                // Check if the report belongs to them
                var missingReport = context.MissingItemData.FirstOrDefault(x => x.ID == missingID && x.submitterUsername.ToLower() == username.ToLower());
                if (missingReport != null)
                {
                    // If the missing item report exists (aka it belongs to them), get the public actions
                    actionsList = actionsList.Where(x => x.isPublic);
                }
                else
                {
                    // If the missing report doesn't exist for this user
                    throw new UnauthorizedAccessException();
                }
            }

            // Typecast into the viewModel and return
            return actionsList.Select(x => (ActionsTakenViewModel)x);
        }
    }
}
