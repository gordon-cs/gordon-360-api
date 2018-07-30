using System.Xml.Linq;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using Gordon360.Models;
using Newtonsoft.Json.Linq;

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

        // All public account info
        public static IEnumerable<PublicStudentProfileViewModel> PublicStudentData { get; set; }
        public static IEnumerable<PublicFacultyStaffProfileViewModel> PublicFacultyStaffData { get; set; }
        public static IEnumerable<PublicAlumniProfileViewModel> PublicAlumniData { get; set; }
        public static IEnumerable<JObject> AllPublicAccounts { get; set; }
        public static IEnumerable<JObject> AllPublicAccountsWithoutAlumni { get; set; }
        public static IEnumerable<JObject> AllPublicAccountsWithoutCurrentStudents { get; set; }

        // All account info
        public static IEnumerable<Student> StudentData { get; set; }
        public static IEnumerable<FacStaff> FacultyStaffData { get; set; }
        public static IEnumerable<Alumni> AlumniData { get; set; }

    }
}