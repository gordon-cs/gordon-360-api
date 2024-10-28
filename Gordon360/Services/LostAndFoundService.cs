using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;


namespace Gordon360.Services
{
    public class LostAndFoundService(CCTContext context) : ILostAndFoundService
    {
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails)
        {
            var newReportResults = context.Missing.Add(new Missing { firstName = reportDetails.firstName, lastName = reportDetails.lastName, category = reportDetails.category, brand = reportDetails.brand, description = reportDetails.description, locationLost = reportDetails.locationLost, stolen = reportDetails.stolen, stolenDescription = reportDetails.stolenDescription, dateLost = reportDetails.dateLost, dateCreated = reportDetails.dateCreated, phoneNumber = reportDetails.phoneNumber, altPhone = reportDetails.altPhone, emailAddr = reportDetails.emailAddr, status = reportDetails.status, adminUsername = reportDetails.adminUsername });
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

        /// <summary>
        /// Get the full list of all missing item reports.
        /// </summary>
        /// <returns>an Enumerable of Missing containing all missing item reports</returns>
        public IEnumerable<Missing> GetMissingItems()
        {
            IEnumerable<Missing> missingList = context.Missing.AsEnumerable();
            return missingList;
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
    }
}
