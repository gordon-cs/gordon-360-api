using System;

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

        public bool stolen { get; set; }

        public string? stolenDescription { get; set; }

        public DateTime dateLost { get; set; }

        public DateTime dateCreated { get; set; }

        public string phoneNumber { get; set; }

        public string? altPhone { get; set; }

        public string emailAddress { get; set; }

        public string status { get; set; }

        public string? adminUsername { get; set; }
    }
}
