using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesUploadViewModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ActivityID { get; set; }
        public int TypeID { get; set; }
        public int? NumberOfTeamsAdmitted { get; set; }
        public int? ScheduleID { get; set; }
    }
}