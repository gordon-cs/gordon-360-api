using System.Xml.Linq;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using Gordon360.Models;

namespace Gordon360.Static.Data
{
    /// <summary>
    /// Service class for data that is shared between all services.
    /// </summary>
    public static class Data
    {
        // XDocument containing the XML data (parsed) from a 25Live URL
        public static XDocument AllEvents { get; set; }

        // Basic Info on Every Account
        public static IEnumerable<BasicInfoViewModel> AllBasicInfo { get; set; }
        // Basic info excluding alumni info
        public static IEnumerable<BasicInfoViewModel> AllBasicInfoWithoutAlumni { get; set; }

        public static IEnumerable<Student> StudentData { get; set; }
        public static IEnumerable<FacStaff> FacultyStaffData { get; set; }
        public static IEnumerable<Alumni> AlumniData { get; set; }

    }
}