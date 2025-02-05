using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class FoundItemViewModel
    {
        public string? recordID { get; set; }

        public string? submitterID { get; set; }

        public string submitterUsername { get; set; }

        public int? matchingMissingID { get; set; }

        public string category { get; set; }

        public string[] colors { get; set; }

        public string? brand { get; set; }

        public string description { get; set; }

        public string locationFound { get; set; }
        
        public DateTime dateFound { get; set; }

        public DateTime dateCreated { get; set; }

        public bool finderWants { get; set; }

        public string status { get; set; }

        public string storageLocation { get; set; }

        public string? finderUsername { get; set; }

        public string? finderFirstName { get; set; }

        public string? finderLastName { get; set; }

        public string? finderPhone { get; set; }

        public string? finderEmail { get; set; }

        public string? ownerUsername { get; set; }

        public string? ownerFirstName { get; set; }

        public string? ownerLastName { get; set; }

        public string? ownerPhone { get; set; }

        public string? ownerEmail { get; set; }

        public IEnumerable<FoundActionsTakenViewModel>? adminActions { get; set; }

        public static implicit operator FoundItemViewModel(CCT.FoundItemData FoundItemDBModel) => new FoundItemViewModel
        {
            recordID = FoundItemDBModel.ID,
            submitterUsername = FoundItemDBModel.submitterUsername,
            matchingMissingID = FoundItemDBModel.matchingMissingID,
            category = FoundItemDBModel.category,
            colors = FoundItemDBModel.colors.Split(',').Select(item => item.Trim()).ToArray(),
            brand = FoundItemDBModel.brand,
            description = FoundItemDBModel.description,
            locationFound = FoundItemDBModel.locationFound,
            dateFound = FoundItemDBModel.dateFound,
            dateCreated = FoundItemDBModel.dateCreated,
            finderWants = FoundItemDBModel.finderWants,
            status = FoundItemDBModel.status,
            storageLocation = FoundItemDBModel.storageLocation,
            finderFirstName = FoundItemDBModel.finderFirstName,
            finderLastName = FoundItemDBModel.finderLastName,
            finderPhone = FoundItemDBModel.finderPhone,
            finderEmail = FoundItemDBModel.finderEmail,
            ownerFirstName = FoundItemDBModel.ownerFirstName,
            ownerLastName = FoundItemDBModel.ownerLastName,
            ownerPhone = FoundItemDBModel.ownerPhone,
            ownerEmail = FoundItemDBModel.ownerEmail,
        };

        // Create a viewmodel from the FoundItemData and the collection of ActionsTaken
        public static FoundItemViewModel From(CCT.FoundItemData FoundItemDBModel, IEnumerable<CCT.FoundActionsTakenData> actionsTaken) => new FoundItemViewModel
        {
            recordID = FoundItemDBModel.ID,
            submitterUsername = FoundItemDBModel.submitterUsername,
            matchingMissingID = FoundItemDBModel.matchingMissingID,
            category = FoundItemDBModel.category,
            colors = FoundItemDBModel.colors.Split(',').Select(item => item.Trim()).ToArray(),
            brand = FoundItemDBModel.brand,
            description = FoundItemDBModel.description,
            locationFound = FoundItemDBModel.locationFound,
            dateFound = FoundItemDBModel.dateFound,
            dateCreated = FoundItemDBModel.dateCreated,
            finderWants = FoundItemDBModel.finderWants,
            status = FoundItemDBModel.status,
            storageLocation = FoundItemDBModel.storageLocation,
            finderFirstName = FoundItemDBModel.finderFirstName,
            finderLastName = FoundItemDBModel.finderLastName,
            finderPhone = FoundItemDBModel.finderPhone,
            finderEmail = FoundItemDBModel.finderEmail,
            ownerFirstName = FoundItemDBModel.ownerFirstName,
            ownerLastName = FoundItemDBModel.ownerLastName,
            ownerPhone = FoundItemDBModel.ownerPhone,
            ownerEmail = FoundItemDBModel.ownerEmail,
            adminActions = actionsTaken.Select(a => (FoundActionsTakenViewModel)a).ToList(),
        };
    }
}
