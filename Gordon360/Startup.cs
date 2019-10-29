using Owin;
using Microsoft.Owin;
using System.Web.Http;
using System.Net.Http.Headers;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;
using Gordon360.AuthorizationServer;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Data;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using System.Xml.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Caching;
using System.Collections.Generic;
using Gordon360.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Gordon360
{
    public class Startup
    {

        public void Configuration(IAppBuilder appBuilder)
        {
            var issuer = System.Web.Configuration.WebConfigurationManager.AppSettings["jwtIssuer"];
            var secret = System.Web.Configuration.WebConfigurationManager.AppSettings["jwtSecret"];

            // Configure Cors
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // Configure options for Authorization Component
            appBuilder.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {

                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                Provider = new TokenIssuer(),
                AccessTokenFormat = new CustomJWTFormat(issuer),
// #if DEBUG
                AllowInsecureHttp = true
// #endif

            });

            // User json web tokens
            appBuilder.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                AllowedAudiences = new[] { issuer },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                }


            });

            // Perform task for the first time at startup
            DoWork();
            DoMoreWork();
            DoMyWork();
            // Register a job(s) in the cache to re-occur at a specified interval
            RegisterCacheEntry();

            // Configure the options for the WebApi Component.
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            appBuilder.UseWebApi(config);
        }

        /// <summary>
        /// Caching task methods created based upon the ideas in the article written by Omar Al Zabir
        /// Article: https://www.codeproject.com/Articles/12117/Simulate-a-Windows-Service-using-ASP-NET-to-run-sc
        /// </summary>

        // Create a new dummy cache entry. We don't want to store anything here, because it will be gone on restart of application
        // Thus, all we need is the frequent callback from this item
        private const string DummyCacheItemKey = "DeloresMichaelLindsay";

        // Register the entry in the cache
        private bool RegisterCacheEntry()
        {
            // Check and see if the dummy entry is already in the cache
            if (null != HttpRuntime.Cache[DummyCacheItemKey])
            {
                return false;
            }

            // Otherwise, we add it to the cache
            HttpRuntime.Cache.Add(DummyCacheItemKey, "Test", null,
                DateTime.MaxValue, TimeSpan.FromMinutes(4),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));
            return true;
        }

        // Perform a job (in this case, we are calling 25Live and storing the data in a "global" variable
        private void DoWork()
        {
            // Make a call to 25Live and retrieve a list of all events
            XDocument _memory = Helpers.GetLiveStream(URLs.ALL_EVENTS_REQUEST);
            // If it is not null, store it to a global variable

            if (_memory != null)
            {
                Data.AllEvents = _memory;
            }
        }

        // Perform a secondary job at startup and at every caching event
        private void DoMoreWork()
        {
            // Make the call to retrieve all basic info on every account
            IEnumerable<BasicInfoViewModel> hold = Helpers.GetAllBasicInfo();
            // If it is not null, store it to a global variable
            if (hold != null)
            {
                Data.AllBasicInfo = hold;
            }
        }

        //Perform a job at startup and at every caching event
        private void DoMyWork()
        {
            //read data from databases and store them in global Data.
            IEnumerable<Student> student = Helpers.GetAllStudent();
            IEnumerable<FacStaff> facstaff = Helpers.GetAllFacultyStaff();
            IEnumerable<Alumni> alumni = Helpers.GetAllAlumni();
            IEnumerable<BasicInfoViewModel> basic = Helpers.GetAllBasicInfoExcludeAlumni();

            IEnumerable<PublicStudentProfileViewModel> publicStudent = Helpers.GetAllPublicStudents();
            IEnumerable<PublicFacultyStaffProfileViewModel> publicFacStaff = Helpers.GetAllPublicFacultyStaff();
            IEnumerable<PublicAlumniProfileViewModel> publicAlumni = Helpers.GetAllPublicAlumni();
            IList<JObject> allPublicAccounts = new List<JObject>();
            IList<JObject> allPublicAccountsWithoutAlumni = new List<JObject>();
            IList<JObject> allPublicAccountsWithoutCurrentStudents = new List<JObject>();

            // storing in global variab
            Data.StudentData = student;
            Data.PublicStudentData = publicStudent;

            Data.FacultyStaffData = facstaff;
            Data.PublicFacultyStaffData = publicFacStaff;

            Data.AlumniData = alumni;
            Data.PublicAlumniData = publicAlumni;
            Data.AllBasicInfoWithoutAlumni = basic;

            foreach (PublicStudentProfileViewModel aStudent in Data.PublicStudentData)
            {
                if(aStudent == null)
                {
                    break;
                }
                JObject theStu = JObject.FromObject(aStudent);
                theStu.Add("Type", "Student");
                theStu.Add("BuildingDescription", null);

                // Get each student's dorm and add it to the collection
                IEnumerable<string> stuBuildAndMail = Gordon360.Services.ComplexQueries.RawSqlQuery<String>.query("SELECT BuildingDescription, Mail_Location, STUDENT.AD_Username from STUDENT join ACCOUNT on (STUDENT.AD_Username = ACCOUNT.AD_Username) where STUDENT.AD_Username = '" + aStudent.AD_Username + "'").Cast<string>();
                string stuBuildDesc = stuBuildAndMail.ElementAt(0);
                string stuMailLoc = stuBuildAndMail.ElementAt(1);
                theStu.Add("Hall", stuBuildDesc);
                theStu.Add("Mail_Location", stuMailLoc);
                theStu.Add("OnCampusDepartment", null);
                allPublicAccounts.Add(theStu);
                allPublicAccountsWithoutAlumni.Add(theStu);
            }
            foreach (PublicFacultyStaffProfileViewModel aFacStaff in Data.PublicFacultyStaffData)
            {
                JObject theFacStaff = JObject.FromObject(aFacStaff);
                theFacStaff.Add("Hall", null);
                theFacStaff.Add("Mail_Location", null);
                theFacStaff.Add("Class", null);
                theFacStaff.Add("Major1Description", null);
                theFacStaff.Add("Major2Description", null);
                theFacStaff.Add("Major3Description", null);
                theFacStaff.Add("Minor1Description", null);
                theFacStaff.Add("Minor2Description", null);
                theFacStaff.Add("Minor3Description", null);
                allPublicAccounts.Add(JObject.FromObject(theFacStaff));
                allPublicAccountsWithoutAlumni.Add(JObject.FromObject(theFacStaff));
                allPublicAccountsWithoutCurrentStudents.Add(JObject.FromObject(theFacStaff));
            }
            foreach (PublicAlumniProfileViewModel anAlum in Data.PublicAlumniData)
            {
                JObject theAlum = JObject.FromObject(anAlum);
                theAlum.Add("Type", "Alum");
                theAlum.Add("BuildingDescription", null);
                theAlum.Add("Hall", null);
                theAlum.Add("Mail_Location", null);
                theAlum.Add("OnCampusDepartment", null);
                theAlum.Add("Class", null);
                theAlum.Add("Major3Description", null);
                theAlum.Add("Minor1Description", null);
                theAlum.Add("Minor2Description", null);
                theAlum.Add("Minor3Description", null);
                allPublicAccounts.Add(theAlum);
                allPublicAccountsWithoutCurrentStudents.Add(theAlum);
            }
            Data.AllPublicAccountsWithoutCurrentStudents = allPublicAccountsWithoutCurrentStudents;
            Data.AllPublicAccounts = allPublicAccounts;
            Data.AllPublicAccountsWithoutAlumni = allPublicAccountsWithoutAlumni;
        }

        // Inside the callback we do all the service work
        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            // Record that the callback works (output to debug console)
            Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());
            // Call the jobs you want to 
            DoWork();
            DoMoreWork();
            DoMyWork();
            // Re-register the item in the cache
            RegisterCacheEntry();
        }
    }

}
