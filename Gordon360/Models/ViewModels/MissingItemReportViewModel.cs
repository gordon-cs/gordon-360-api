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

        public string[]? colors { get; set; }

        public string? brand { get; set; }

        public string description { get; set; }

        public string locationLost { get; set; }

        public bool? stolen { get; set; }

        public string? stolenDescription { get; set; }

        public DateTime? dateLost { get; set; }

        public DateTime? dateCreated { get; set; }

        public string phoneNumber { get; set; }

        public string? altPhone { get; set; }

        public string emailAddr { get; set; }

        public string status { get; set; }

        public string? adminUsername { get; set; }

        public static implicit operator MissingItemReportViewModel(CCT.Missing MissingReportDBModel) => new MissingItemReportViewModel
        {
            recordID = MissingReportDBModel.recordID,
            firstName = MissingReportDBModel.firstName,
            lastName = MissingReportDBModel.lastName,
            category = MissingReportDBModel.category,
            brand = MissingReportDBModel.brand,
            description = MissingReportDBModel.description,
            locationLost = MissingReportDBModel.locationLost,
            stolen = MissingReportDBModel.stolen,
            stolenDescription = MissingReportDBModel.stolenDescription,
            dateLost = MissingReportDBModel.dateLost,
            dateCreated = MissingReportDBModel.dateCreated,
            phoneNumber = MissingReportDBModel.phoneNumber,
            altPhone = MissingReportDBModel.altPhone,
            emailAddr = MissingReportDBModel.emailAddr,
            status = MissingReportDBModel.status,
            adminUsername = MissingReportDBModel.adminUsername,
        };
    }
}
