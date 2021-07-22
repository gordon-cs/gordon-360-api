using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AcademicCheckInViewModel
    {
        public string Holds { get; set; }

        public string PersonalPhone { get; set; }

        public bool MakePrivate { get; set; }

        public bool NoPhone { get; set; }

        public int Ethnicity { get; set; }

        public String Race { get; set; }
        public Boolean FinancialHold { get; set; }
        public Boolean HighSchoolHold { get; set; }
        public Boolean MedicalHold { get; set; }
        public Boolean MajorHold { get; set; }
        public Boolean RegistrarHold { get; set; }
        public Boolean LaVidaHold { get; set; }
        public Boolean MustRegisterForClasses { get; set; }
        public Int32 NewStudent { get; set; }
        public String FinancialHoldText { get; set; }
        public String MeetingDate { get; set; }
        public String MeetingLocations { get; set; }
        public Boolean FinalizationCompleted { get; set; } //PLACEHOLDER FOR VALUE PASSED FROM DB
    }
}
