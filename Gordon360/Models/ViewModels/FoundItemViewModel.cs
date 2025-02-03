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

        public string finderFirstName { get; set; }

        public string finderLastName { get; set; }

        public string finderPhone { get; set; }

        public string finderEmail { get; set; }

        public string ownerFirstName { get; set; }

        public string ownerLastName { get; set; }

        public string ownerPhone { get; set; }

        public string ownerEmail { get; set; }

        public IEnumerable<FoundActionsTakenViewModel>? adminActions { get; set; }

        public static implicit operator FoundItemViewModel(CCT.FoundItemData MissingReportDBModel) => new FoundItemViewModel
        {
            recordID = MissingReportDBModel.ID,
            category = MissingReportDBModel.category,
            colors = MissingReportDBModel.colors.Split(',').Select(item => item.Trim()).ToArray(),
            brand = MissingReportDBModel.brand,
            description = MissingReportDBModel.description,
            locationFound = MissingReportDBModel.locationFound,
            dateFound = MissingReportDBModel.dateFound,
            dateCreated = MissingReportDBModel.dateCreated,
            status = MissingReportDBModel.status,
            submitterUsername = MissingReportDBModel.submitterUsername,
        };
    }
}
