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
    public class LostAndFoundService(CCTContext context, IAccountService accountService) : ILostAndFoundService
    {
        /// <summary>
        /// Check if the user has full admin permissions in the system
        /// </summary>
        /// <param name="username">the UPN of the user</param>
        /// <returns></returns>
        private bool hasFullPermissions(string username)
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
                Log.Error("NoMatchingPrincipleException encountered and handled when searching for LostAndFoundDevelopers group on USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isDev = false;
            }
            try
            {
                isAdmin = userGroups.Contains(Enums.AuthGroup.LostAndFoundAdmin);
            }
            catch (NoMatchingPrincipalException e)
            {
                Log.Error("NoMatchingPrincipleException encountered and handled when searching for LostAndFoundAdmin group on USER UPN " + username + " EXCEPTION: " + e);
                // If we fail to get the admin group, default to false.
                isAdmin = false;
            }
            return (isAdmin || isDev);
        }

        /// <summary>
        /// Create a new missing item report, for the submitter in the details, or the authenticated user if that is null.
        /// </summary>
        /// <param name="reportDetails"></param>
        /// <param name="username"></param>
        /// <returns>Report ID - The ID of the report, generated by the database.</returns>
        /// <exception cref="ResourceCreationException">If a general user attempts to create a report for someone else
        /// or the report can't be successfully saved</exception>
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails, string username)
        {
            // If a general user attempts to create a missing item report for someone else, or for a guest user
            if (!hasFullPermissions(username) && (reportDetails.submitterUsername.ToLower() != username.ToLower() || reportDetails.forGuest))
            {
                throw new ResourceCreationException() { ExceptionMessage = "Cannot create missing item report for someone else." };
            }

            // Get the id for the username submitted with the report object
            string idNum;
            if (reportDetails.submitterUsername != "")
            {
                // By default, try to get the account of the username in the
                idNum = accountService.GetAccountByUsername(reportDetails.submitterUsername).GordonID;
            }
            else
            {
                idNum = accountService.GetAccountByUsername(username).GordonID;
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
        /// Create an action taken for the missing item report with given id
        /// </summary>
        /// <param name="missingItemID">The id of the missing item to add an action to</param>
        /// <param name="ActionsTaken">The actions taken object to create</param>
        /// <param name="username">The username of the person making the request</param>
        /// <returns>actionTakenID - The ID of the action taken, generated by the database.</returns>
        /// <exception cref="ResourceCreationException">If the requesting users account can't be found, or the action fails to save</exception>
        /// <exception cref="ResourceNotFoundException">If no missing item report exists with the given id</exception>
        /// <exception cref="UnauthorizedAccessException">If a non-admin user attempts to create an action on a report which doesn't
        /// belong to them</exception>
        public int CreateActionTaken(int missingItemID, ActionsTakenViewModel ActionsTaken, string username)
        {
            // Get requesting user's ID number
            var idNum = accountService.GetAccountByUsername(username).GordonID;

            var missingItemReport = context.MissingReports.Find(missingItemID);
            if (missingItemReport == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No missing item report with given id found in the system" };
            }

            // If the reports does not belong to the user, and the user is not an admin
            if (missingItemReport.submitterID != idNum && !hasFullPermissions(username)) 
            {
                throw new UnauthorizedAccessException("Cannot modify a report that doesn't belong to you!");
            }

            var newActionTaken = context.ActionsTaken.Add(new ActionsTaken
            {
                missingID = missingItemID,
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
        ///    NOTE: cannot modify associated guest user data, if this report is for guest.
        /// </summary>
        /// <param name="missingItemID">The id of the missing item to modify</param>
        /// <param name="reportDetails">The new object to update to</param>
        /// <param name="username">The username of the person making the request</param>
        /// <returns>None</returns>
        /// <exception cref="ResourceCreationException">If not account can be found for the requesting user</exception>
        /// <exception cref="ResourceNotFoundException">If the missing item report with given id cannot be found in the database</exception>
        /// <exception cref="UnauthorizedAccessException">If the report to be modified doesn't belong to the requesting user 
        /// (admins cannot edit reports of other people)</exception>
        public async Task UpdateMissingItemReportAsync(int missingItemID, MissingItemReportViewModel reportDetails, string username)
        {
            // Get requesting user's ID number
            var idNum = accountService.GetAccountByUsername(username).GordonID;

            var original = await context.MissingReports.FindAsync(missingItemID);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Missing Item Report was not found" };
            }

            // If the report doesn't belong to the requesting user
            if (original.submitterID != idNum)
            {
                throw new UnauthorizedAccessException("Cannot modify a report that doesn't belong to you!");
            }

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

        /// <summary>
        /// Update the status of a report with given id, to the given status message
        ///     Status text must be in the set of allowed statuses, "Active", "Expired", "Deleted", "Found"
        /// </summary>
        /// <param name="missingItemID">The id of the missing item to modify</param>
        /// <param name="status">The new status</param>
        /// <param name="username">The username of the person making the request</param>
        /// <returns>None</returns>
        /// <exception cref="ResourceCreationException">If not account can be found for the requesting user</exception>
        /// <exception cref="ResourceNotFoundException">If the missing item report with given id cannot be found in the database</exception>
        /// <exception cref="UnauthorizedAccessException">If the report to be modified doesn't belong to the requesting user and the
        /// user is not an admin</exception>
        public async Task UpdateReportStatusAsync(int missingItemID, string status, string username)
        {
            // Get requesting user's ID number
            var idNum = accountService.GetAccountByUsername(username).GordonID;

            var original = await context.MissingReports.FindAsync(missingItemID);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Missing Item Report was not found" };
            }
            
            // If a non-admin user attempts to update the status of a report
            if (original.submitterID != idNum && !hasFullPermissions(username))
            {
                throw new UnauthorizedAccessException("Cannot modify a report that doesn't belong to you!");
            }

            original.status = status;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get the list of missing item reports for given user.
        /// </summary>
        /// <param name="requestedUsername">The username to get the data of, if allowed</param>
        /// <param name="requestorUsername">The username of the person making the request</param>
        /// <returns>an Enumerable of Missing Item Reports containing all missing item reports</returns>
        /// <exception cref="ResourceNotFoundException">If a user requests reports they are not permitted to access</exception>
        public IEnumerable<MissingItemReportViewModel> GetMissingItems(string requestedUsername, string requestorUsername)
        {
            if (hasFullPermissions(requestorUsername))
            {
                // If this is an admin user, get the reports for the requested username
                return context.MissingItemData
               .Where(x => x.submitterUsername == requestedUsername && !x.forGuest)
               .GroupJoin(context.ActionsTakenData
               .Where(x => x.isPublic),
                   missingItem => missingItem.ID,
                   action => action.missingID,
                   (missingItem, action) => MissingItemReportViewModel.From(missingItem, action));
            }
            else
            {
                // If a non-admin user requests the reports of someone else
                if (!requestorUsername.Equals(requestedUsername, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "No missing item reports could be found" };
                }

                return context.MissingItemData
               .Where(x => x.submitterUsername == requestedUsername && !x.forGuest)
               .GroupJoin(context.ActionsTakenData
               .Where(x => x.isPublic),
                   missingItem => missingItem.ID,
                   action => action.missingID,
                   (missingItem, action) => MissingItemReportViewModel.From(missingItem, action));

            }
        }

        /// <summary>
        /// Get all missing item reports
        /// Throw unauthorized access exception if the user doesn't have admin permissions
        /// </summary>
        /// <param name="color">The selected color for filtering reports</param>
        /// <param name="category">The selected category for filtering reports</param>
        /// <param name="keywords">The selected keywords for filtering by keywords</param>
        /// <param name="status">The selected status for filtering reports</param>
        /// <param name="username">The username of the person making the request</param>
        /// <returns>An enumerable of Missing Item Reports, from the Missing Item Data view</returns>
        /// <exception cref="UnauthorizedAccessException">If a user without admin permissions attempts to use</exception>
        public IEnumerable<MissingItemReportViewModel> GetMissingItemsAll(string username, string? status, string? color, string? category, string? keywords)
        {
            if (!hasFullPermissions(username))
            {
                throw new UnauthorizedAccessException();
            }

            IQueryable<MissingItemData> missingItems = context.MissingItemData;
            if (status is not null)
            {
                missingItems = missingItems.Where(x => x.status == status);
            }
            if (color is not null)
            {
                missingItems = missingItems.Where(x => x.colors.Contains(color));
            }
            if (category is not null) 
            { 
                missingItems = missingItems.Where(x => x.category == category);
            }
            if (keywords is not null) 
            {
                missingItems = missingItems.Where(x => x.firstName.Contains(keywords) 
                                                    || x.lastName.Contains(keywords) 
                                                    || x.description.Contains(keywords) 
                                                    || x.locationLost.Contains(keywords));
            }

            // Perform a group join to create a MissingItemReportViewModel with actions taken data for each report
            // Only performs a single SQL query to the db, so much more performant than alternative solutions
            return missingItems
                      .GroupJoin(context.ActionsTakenData.OrderBy(action => action.actionDate),
                          missingItem => missingItem.ID,
                          action => action.missingID,
                          (missingItem, action) => MissingItemReportViewModel.From(missingItem, action));
        }

        /// <summary>
        /// Gets a Missing by id, only allowed if it belongs to the username, or the user is an admin
        /// </summary>
        /// <param name="missingItemID">The ID of the missing item</param>
        /// <param name="username">The username of the person making the request</param>
        /// <returns>A Missing Item Report object, or null if no item matches the id</returns>
        /// <exception cref="ResourceNotFoundException">If the report with given ID doesn't exist or the user
        /// doesn't have permissions to read it</exception>
        public MissingItemReportViewModel? GetMissingItem(int missingItemID, string username)
        {
            MissingItemReportViewModel report;
            // If user is admin or developer, simply get the report
            if (hasFullPermissions(username))
            {
                var data = context.MissingItemData.FirstOrDefault(x => x.ID == missingItemID);
                if (data != null)
                {
                    report = (MissingItemReportViewModel)data;

                    // Get the list of all admin actions on this report, and add them to the report.
                    report.adminActions = GetActionsTaken(missingItemID, username, false, true);
                }
                else
                {
                    // If no such report exists
                    throw new ResourceNotFoundException();
                }
            }
            else
            {
                // Otherwise get the report if it belongs to the requesting user
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
        /// Gets a list of actions taken on a missing item report with given ID, 
        /// general users only allowed to get public actions on their own reports
        /// </summary>
        /// <param name="missingID">The ID of the missing item report to get the actions of</param>
        /// <param name="username">The username of the person making the request</param>
        /// <param name="getPublicOnly">Only get actions marked as public.  Default false.</param>
        /// <param name="elevatedPermissions">Signal to the function that user elevated authorization has already been confirmed</param>
        /// <returns>An ActionsTaken[], or null if no item matches the id</returns>
        /// <exception cref="ResourceNotFoundException">Attemps by a non-admin user to get actions for 
        /// a report which doesn't belong to them will yield an ResourceNotFoundException</exception>
        public IEnumerable<ActionsTakenViewModel> GetActionsTaken(int missingID, string username, bool getPublicOnly = false, bool elevatedPermissions = false)
        {
            // Ignore checking authorization if authorization is set (improved performance for large admin requests)
            if (!elevatedPermissions)
            {
                elevatedPermissions = hasFullPermissions(username);
            }

            // Get all actions taken for the report with given
            IQueryable<ActionsTakenData> actionsList = context.ActionsTakenData.Where(x => x.missingID == missingID);

            // If an admin requests only public actions
            if (elevatedPermissions && getPublicOnly)
            {
                actionsList = actionsList.Where(x => x.isPublic);
            }
            // Otherwise if a general user requests actions for a report
            else if (!elevatedPermissions)
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
                    // If the missing report doesn't belong to this user, short circuit and throw exception
                    throw new ResourceNotFoundException();
                }
            }

            // Typecast into the viewModel and return
            return actionsList.Select(x => (ActionsTakenViewModel)x);
        }
    }
}
