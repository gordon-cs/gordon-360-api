using System;
using System.Linq;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
    // Backend model of a missing item report
    public class MissingItemReportViewModel
    {
        public int? recordID { get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string category { get; set; }

        public string[] colors { get; set; }

        public string? brand { get; set; }

        public string description { get; set; }

        public string locationLost { get; set; }

        public bool stolen { get; set; }

        public string? stolenDescription { get; set; }

        public DateTime dateLost { get; set; }

        public DateTime dateCreated { get; set; }

        public string? phone { get; set; }

        public string? email { get; set; }

        public string status { get; set; }

        public string submitterUsername { get; set; }

        public string? submitterID { get; set; } //ID used internally, but will never be passed to the frontend

        public bool forGuest { get; set; }

        public IEnumerable<ActionsTakenViewModel>? adminActions { get; set; }

        public static implicit operator MissingItemReportViewModel(CCT.MissingItemData MissingReportDBModel) => new MissingItemReportViewModel
        {
            recordID = MissingReportDBModel.ID,
            firstName = MissingReportDBModel.firstName,
            lastName = MissingReportDBModel.lastName,
            category = MissingReportDBModel.category,
            colors = MissingReportDBModel.colors.Split(',').Select(item => item.Trim()).ToArray(),
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
            forGuest = MissingReportDBModel.forGuest,
        };
    }
}
