using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Gordon360.Static.Data
{
    /// <summary>
    /// Service class for data that is shared between all services.
    /// </summary>
    public static class Data
    {


        // All public account info
        public static IEnumerable<JObject> AllPublicStudentAccounts { get; set; }
        public static IEnumerable<JObject> AllPublicFacStaffAccounts { get; set; }
        public static IEnumerable<JObject> AllPublicAlumniAccounts { get; set; }

    }
}