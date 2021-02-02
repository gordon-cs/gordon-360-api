using System;

namespace Gordon360.Models.ViewModels
{
    public class ApartmentApplicantDetailsViewModel
    {
        public int AprtAppID { get; set; }
        public string EditorID { get; set; }
        public DateTime? DateSubmitted { get; set; } // Nullable
        public DateTime DateModified { get; set; }
        public string ID_NUM { get; set; }
        public string AprtProgram { get; set; } // Nullable
        public Boolean? AprtProgramCredit { get; set; } // Nullable
        public string SESS_CDE { get; set; }
    }
}