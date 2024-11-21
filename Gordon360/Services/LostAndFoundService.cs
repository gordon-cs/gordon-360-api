using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Gordon360.Services
{
    public class LostAndFoundService(CCTContext context) : ILostAndFoundService
    {
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails, string username)
        {

            var account = context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

            string idNum;

            if (account != null)
            {
                idNum = account.gordon_id;
            }
            else
            {
                throw new ResourceCreationException() { ExceptionMessage = "No account could be found for the user." };
            }

            // Create the new report
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

            // Add a guest user for this report, if submitted for a guest user.
            if (reportDetails.forGuest)
            {
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

        /// <param name="id">The id</param>
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
                submitterID = idNum
            });

            context.SaveChanges();

            if (newActionTaken == null || newActionTaken?.Entity?.ID == 0)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The action could not be saved." };
            }

            int actionTakenID = newActionTaken.Entity.ID;

            return actionTakenID;
        }

        /// <param name="id">The id</param>
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

        /// <param name="id">The id</param>
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
        /// <returns>an Enumerable of Missing containing all missing item reports</returns>
        public IEnumerable<MissingItemReportViewModel> GetMissingItems(string username)
        {
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

            IEnumerable<MissingItemData> missingList = context.MissingItemData.Where(x => x.submitterID == idNum && x.forGuest == false);
            IEnumerable<MissingItemReportViewModel> returnList = missingList.Select(x => (MissingItemReportViewModel)x);
            return returnList;
        }

        public IEnumerable<MissingItemReportViewModel> GetMissingItemsAll()
        {
            IEnumerable<MissingItemData> missingList = context.MissingItemData.AsEnumerable();
            return missingList.Select(x => (MissingItemReportViewModel)x);
        }

        /// <summary>
        /// Gets all FoundItems
        /// </summary>
        /// <returns>An Enumerable of FoundItems containing all the Found Items</returns>
        public IEnumerable<FoundItems> GetFoundItems()
        {
            return context.FoundItems.AsEnumerable();
        }

        /// <summary>
        /// Gets a FoundItem by id
        /// </summary>
        /// <param name="ID">The ID of the found item</param>
        /// <returns>A FoundItem, or null if no item matches the id</returns>
        public FoundItems? GetFoundItem(int ID)
        {
            return context.FoundItems.FirstOrDefault(x => x.ID == ID);
        }

        /// <summary>
        /// Gets a Missing by id
        /// </summary>
        /// <param name="id">The ID of the missing item</param>
        /// <returns>A Missing, or null if no item matches the id</returns>
        public MissingItemReportViewModel? GetMissingItem(int id)
        {
            MissingItemData report = context.MissingItemData.FirstOrDefault(x => x.ID == id);
            return (MissingItemReportViewModel)report;
        }

        /// <summary>
        /// Gets actions taken by id
        /// </summary>
        /// <param name="id">The ID of the associated missing item report</param>
        /// <returns>An ActionsTaken, or null if no item matches the id</returns>
        public IEnumerable<ActionsTakenViewModel> GetActionsTaken(int id)
        {
            IEnumerable<ActionsTaken> actionsList = context.ActionsTaken.Where(x => x.missingID == id);
            IEnumerable<ActionsTakenViewModel> returnList = actionsList.Select(x => (ActionsTakenViewModel)x);
            return returnList;
        }
    }
}
