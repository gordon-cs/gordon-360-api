using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Services
{
    public class LostAndFoundService(CCTContext context) : ILostAndFoundService
    {
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails)
        {
            var newReportResults = context.Missing.Add(new Missing { 
                firstName = reportDetails.firstName, 
                lastName = reportDetails.lastName, 
                category = reportDetails.category,
                brand = reportDetails.brand, 
                description = reportDetails.description,
                locationLost = reportDetails.locationLost, 
                stolen = reportDetails.stolen, 
                stolenDescription = reportDetails.stolenDescription, 
                dateLost = reportDetails.dateLost, 
                dateCreated = reportDetails.dateCreated, 
                phoneNumber = reportDetails.phone, 
                emailAddr = reportDetails.email, 
                status = reportDetails.status,
                adminUsername = reportDetails.submitterUsername });
            // var newReportResults = context.Missing.Add(new Missing(reportDetails))

            context.SaveChanges();

            if (newReportResults?.Entity == null || newReportResults?.Entity?.recordID == 0)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The application could not be saved." };
            }

            // Retrieve the application ID number of this new application
            int reportID = newReportResults.Entity.recordID;

            return reportID;
        }

        /// <param name="id">The id</param>
        public async Task UpdateMissingItemReportAsync(int id, MissingItemReportViewModel reportDetails)
        {
            var original = await context.Missing.FindAsync(id);

            if (original != null)
            {
                original.firstName = reportDetails.firstName;
                original.lastName = reportDetails.lastName;
                original.category = reportDetails.category;
                original.brand = reportDetails.brand;
                original.description = reportDetails.description;
                original.locationLost = reportDetails.locationLost;
                original.stolen = reportDetails.stolen;
                original.stolenDescription = reportDetails.stolenDescription;
                original.dateLost = reportDetails.dateLost;
                original.dateCreated = reportDetails.dateCreated;
                original.phoneNumber = reportDetails.phone;
                original.emailAddr = reportDetails.email;
                original.status = reportDetails.status;
                original.adminUsername = reportDetails.submitterUsername;

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
            var original = await context.Missing.FindAsync(id);

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
        /// Get the full list of all missing item reports.
        /// </summary>
        /// <returns>an Enumerable of Missing containing all missing item reports</returns>
        public IEnumerable<MissingItemReportViewModel> GetMissingItems()
        {
            IEnumerable<MissingItemData> missingList = context.MissingItemData.AsEnumerable();
            IEnumerable<MissingItemReportViewModel> returnList = missingList.Select(x => (MissingItemReportViewModel)x);
            return returnList;
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
        public Missing? GetMissingItem(int id)
        {
            return context.Missing.FirstOrDefault(x => x.recordID == id);
        }
    }
}
