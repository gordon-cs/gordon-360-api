using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AcademicCheckInViewModel
    {
        public string Status { get; set; }

        public DateTime Created { get; set; }

        public bool IsValid { get; set; }
    }
}