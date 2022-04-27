using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gordon360.Static.Methods
{
    /// <summary>
    /// Service class for methods that are shared between all services.
    /// </summary>
    public static class Helpers
    {
        private static CCTContext Context => new();

        //public static IEnumerable<Student> GetAllStudent()
        //{
        //    return Context.Student.Where(s => s.AD_Username != null);
        //}

        //public static JObject GetAllPublicStudents()
        //{
        //    // JSON string result
        //    IEnumerable<string> fragmentedString = null;
        //    string result = null;
        //    JObject publicStudentData = null;
        //    try
        //    {
        //        // Attempt to query the DB
        //        fragmentedString = RawSqlQuery<string>.query(SQLQuery.ALL_PUBLIC_STUDENT_REQUEST).Cast<string>();
        //        foreach (string fragment in fragmentedString)
        //        {
        //            result = result + fragment;
        //        }
        //        publicStudentData = JObject.Parse(result);
        //    }
        //    catch (Exception e)
        //    {
        //        //
        //        Debug.WriteLine("Failed to parse JSON data:");
        //        Debug.WriteLine(e.Message);
        //    }
        //    // Filter out results with null or empty active directory names
        //    return publicStudentData;
        //}

        //public static IEnumerable<FacStaff> GetAllFacultyStaff()
        //{
        //    return Context.FacStaff.Where(fs => fs.AD_Username != null);
        //}

        //public static IEnumerable<PublicFacultyStaffProfileViewModel> GetAllPublicFacultyStaff()
        //{
        //    // Create a list to be filled
        //    IEnumerable<PublicFacultyStaffProfileViewModel> result = null;

        //    try
        //    {
        //        // Attempt to query the DB
        //        result = RawSqlQuery<PublicFacultyStaffProfileViewModel>.query(SQLQuery.ALL_PUBLIC_FACULTY_STAFF_REQUEST);
        //    }
        //    catch
        //    {
        //        //
        //    }
        //    // Filter out results with null or empty active directory names
        //    return result;
        //}

        //public static IEnumerable<PublicAlumniProfileViewModel> GetAllPublicAlumni()
        //{
        //    // Create a list to be filled
        //    IEnumerable<PublicAlumniProfileViewModel> result = null;

        //    try
        //    {
        //        // Attempt to query the DB
        //        result = RawSqlQuery<PublicAlumniProfileViewModel>.query(SQLQuery.ALL_PUBLIC_ALUMNI_REQUEST);
        //    }
        //    catch
        //    {
        //        //
        //    }
        //    // Filter out results with null or empty active directory names
        //    return result;
        //}

        //public static IEnumerable<Alumni> GetAllAlumni()
        //{
        //    return Context.Alumni.Where(s => s.AD_Username != null);
        //}

        //public static IEnumerable<BasicInfoViewModel> GetAllBasicInfoExcludeAlumni()
        //{
        //    IEnumerable<BasicInfoViewModel> result = null;
        //    try
        //    {
        //        // Attempt to query the DB
        //        result = RawSqlQuery<BasicInfoViewModel>.query(SQLQuery.ALL_BASIC_INFO_NOT_ALUM);
        //    }
        //    catch
        //    {
        //        //
        //    }
        //    // Filter out results with null or empty active directory names
        //    return result;
        //}

        //// Get all basic info without Alumni
        //public static IEnumerable<BasicInfoViewModel> GetBasicInfoWithoutAlumni( IEnumerable<AlumniProfileViewModel> alumni, IEnumerable<BasicInfoViewModel> basic)
        //{
        //    IEnumerable<BasicInfoViewModel> result = null;
        //    result = basic.Where(b => !alumni.Any(a => a.AD_Username == b.UserName));
        //    return result;
        //}

        /// <summary>
        /// Service method that gets the current session we are in.
        /// </summary>
        /// <returns>SessionViewModel of the current session. If no session is found for our current date, returns null.</returns>
        public static async Task<SessionViewModel> GetCurrentSessionAsync()
        {
            // TODO: Pass CCTEntities context by configuration/options from startup
            var context = new CCTContext();
            var sessionService = new SessionService(context);

            var query = await context.Procedures.CURRENT_SESSIONAsync();
            var currentSessionCode = query.Select(x => x.DEFAULT_SESS_CDE).FirstOrDefault();

            return sessionService.Get(currentSessionCode);
        }

        // Return the first day in the current session
        public static String GetFirstDay()
        {
            var currentSession = await GetCurrentSessionAsync();
            DateTime firstDayRaw = currentSession.SessionBeginDate.Value;
            string firstDay = firstDayRaw.ToString("MM/dd/yyyy");
            return firstDay;
        }

        // Return the last day in the current session
        public static async Task<string> GetLastDayAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            DateTime lastDayRaw = currentSession.SessionEndDate.Value;
            string lastDay = lastDayRaw.ToString("MM/dd/yyyy");
            return lastDay;
        }

        // Return the days left in the semester, and the total days in the current session
        public static async Task<double[]> GetDaysLeftAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            // The end of the current session
            DateTime sessionEnd = currentSession.SessionEndDate.Value;
            DateTime sessionBegin = currentSession.SessionBeginDate.Value;
            // Get todays date
            DateTime startTime = DateTime.Today;
            //Initialize array
            double[] days = new double[2];
            // Days left in semester
            days[0] = (sessionEnd - startTime).TotalDays;
            // Total days in the semester
            days[1] = (sessionEnd - sessionBegin).TotalDays;
            return days;
        }

        //// Fill an iterable list of basicinfo from a query to the database
        //public static IEnumerable<BasicInfoViewModel> GetAllBasicInfo()
        //{
        //    // Create a list to be filled
        //    IEnumerable<BasicInfoViewModel> result = null;
        //    try
        //    {
        //        // Attempt to query the DB
        //        result = RawSqlQuery<BasicInfoViewModel>.query("ALL_BASIC_INFO");
        //    }
        //    catch
        //    {
        //        //
        //    }
        //    // Filter out results with null or empty active directory names
        //    return result;
        //}

        public static string GetLeaderRoleCodes()
        {
            return "LEAD";
        }

        public static string GetAdvisorRoleCodes()
        {
            return "ADV";
        }

        public static string GetTranscriptWorthyRoles()
        {
            //string[] transcriptWorthyRoles = { "CAPT", "CODIR", "CORD", "DIREC", "PRES", "VICEC", "VICEP", "AC", "RA1", "RA2","RA3", "SEC" };
            return "LEAD";
        }


        // For goStalk/Advanced People Search:

        // Fill an iterable list of majors from a query to the database
        public static IEnumerable<string> GetMajors()
        {

            return Context.Majors.OrderBy(m => m.MajorDescription)
                                 .Select(m => m.MajorDescription)
                                 .Distinct();
        }

        // Fill an iterable list of minors from a query to the database
        public static IEnumerable<string> GetMinors()
        {
            return Context.Student.Select(s => s.Minor1Description)
                                  .Distinct()
                                  .Where(s => s != null);
        }

        // Fill an iterable list of halls from a query to the database
        public static IEnumerable<string> GetHalls()
        {
            return Context.Student.Select(s => s.BuildingDescription)
                                  .Distinct()
                                  .Where(b => b != null)
                                  .OrderBy(b => b);
        }

        // Fill an iterable list of states from a query to the database
        public static IEnumerable<string> GetStates()
        {
            var context = Context;
            return context.Student.Select(s => s.HomeState).AsEnumerable()
                                  .Union(context.FacStaff.Select(fs => fs.HomeState).AsEnumerable())
                                  .Union(context.Alumni.Select(a => a.HomeState).AsEnumerable())
                                  .Distinct()
                                  .Where(s => s != null)
                                  .OrderBy(s => s);
        }

        // Fill an iterable list of countries from a query to the database
        public static IEnumerable<string> GetCountries()
        {
            var context = Context;
            return context.Student.Select(s => s.Country).AsEnumerable()
                                  .Union(context.FacStaff.Select(fs => fs.Country).AsEnumerable())
                                  .Union(context.Alumni.Select(a => a.Country).AsEnumerable())
                                  .Distinct()
                                  .Where(s => s != null)
                                  .OrderBy(s => s);
        }

        // Fill an iterable list of departments from a query to the database
        public static IEnumerable<string> GetDepartments()
        {
            return Context.FacStaff.Select(fs => fs.OnCampusDepartment)
                                   .Distinct()
                                   .Where(d => d != null)
                                   .OrderBy(d => d);
        }

        // Fill an iterable list of buildings from a query to the database
        public static IEnumerable<string> GetBuildings()
        {
            return Context.FacStaff.Select(fs => fs.BuildingDescription)
                                   .Distinct()
                                   .Where(d => d != null)
                                   .OrderBy(d => d);
        }
    }
}
