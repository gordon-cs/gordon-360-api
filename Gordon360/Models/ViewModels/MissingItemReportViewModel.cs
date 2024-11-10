using Gordon360.Models.CCT;
using System;
using System.Reflection;

namespace Gordon360.Models.ViewModels
{
    public class MissingItemReportViewModel
    {
        public int? recordID { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string category { get; set; }

        public string? colors { get; set; }

        public string? brand { get; set; }

        public string description { get; set; }

        public string locationLost { get; set; }

        public bool stolen { get; set; }

        public string? stolenDescription { get; set; }

        public DateTime? dateLost { get; set; }

        public DateTime? dateCreated { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string status { get; set; }

        public string submitterUsername { get; set; }

        public static implicit operator MissingItemReportViewModel(CCT.MissingItemData MissingReportDBModel) => new MissingItemReportViewModel
        {
            recordID = MissingReportDBModel.ID,
            firstName = MissingReportDBModel.firstName,
            lastName = MissingReportDBModel.lastName,
            category = MissingReportDBModel.category,
            brand = MissingReportDBModel.brand,
            description = MissingReportDBModel.description,
            locationLost = MissingReportDBModel.locationLost,
            stolen = MissingReportDBModel.stolen,
            stolenDescription = MissingReportDBModel.stolenDesc,
            dateLost = MissingReportDBModel.dateLost,
            dateCreated = MissingReportDBModel.dateCreated,
            phone = MissingReportDBModel.phone,
            email = MissingReportDBModel.email,
            status = MissingReportDBModel.status,
            submitterUsername = MissingReportDBModel.submitterUsername,
        };
    }
}
