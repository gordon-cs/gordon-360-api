using System.Xml.Linq;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
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
        public static IEnumerable<StudentProfileViewModel> StudentData { get; set; }
        public static IEnumerable<FacultyStaffProfileViewModel> FacultyStaffData { get; set; }
        public static IEnumerable<AlumniProfileViewModel> AlumniData { get; set; }

    }
}