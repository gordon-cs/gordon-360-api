using Owin;
using Microsoft.Owin;
using System.Web.Http;
using System.Net.Http.Headers;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;
using Gordon360.Services.ComplexQueries;
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

            JObject publicStudent = Helpers.GetAllPublicStudents();
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

            foreach (JToken aStudent in Data.PublicStudentData.SelectToken("Students"))
            {
                if(aStudent == null)
                {
                    break;
                }
                JObject theStu = JObject.FromObject(aStudent);
                theStu.Add("Type", "Student");
                theStu.Add("BuildingDescription", null);
                
                string firstName = null;
                string lastName = null;
                string nickName = null;
                string mailLocation = null;
                string homeCity = null;
                string homeState = null;
                string stuClass = null;
                string hall = null;
                string keepPrivate = null;
                string email = null;
                string adUsername = null;
                string country = null;
                string major1Description = null;
                string major2Description = null;
                string major3Description = null;
                string minor1Description = null;
                string minor2Description = null;
                string minor3Description = null;

                // If at first you don't succeed...

                try
                {
                    firstName = theStu["FirstName"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("FirstName", null);
                }

                try
                {
                    lastName = theStu["LastName"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("LastName", null);
                }

                try
                {
                    nickName = theStu["NickName"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("NickName", null);
                }

                try
                {
                    mailLocation = theStu["Mail_Location"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Mail_Location", null);
                }

                try
                {
                    homeCity = theStu["HomeCity"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("HomeCity", null);
                }

                try
                {
                    homeState = theStu["HomeState"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("HomeState", null);
                }

                try
                {
                    stuClass = theStu["Class"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Class", null);
                }

                try
                {
                    hall = theStu["Hall"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Hall", null);
                }

                try
                {
                    keepPrivate = theStu["KeepPrivate"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("KeepPrivate", null);
                }

                try
                {
                    email = theStu["Email"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Email", null);
                }

                try
                {
                    adUsername = theStu["AD_Username"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("AD_Username", null);
                }

                try
                {
                    country = theStu["Country"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Country", null);
                }

                try
                {
                    major1Description = theStu["Major1Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Major1Description", null);
                }

                try
                {
                    major2Description = theStu["Major2Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Major2Description", null);
                }

                try
                {
                    major3Description = theStu["Major3Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Major3Description", null);
                }

                try
                {
                    minor1Description = theStu["Minor1Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Minor1Description", null);
                }

                try
                {
                    minor2Description = theStu["Minor2Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Minor2Description", null);
                }

                try
                {
                    minor3Description = theStu["Minor3Description"].ToString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    // If the previous assignment throws an exception, the value does not exist
                    // in the JObject. Add it as a null value.
                    theStu.Add("Minor3Description", null);
                }

                // ...again

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
