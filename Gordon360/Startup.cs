using Owin;
using Microsoft.Owin;
using System.Web.Http;
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

            // storing in global variab
            Data.StudentData = student;
            Data.FacultyStaffData = facstaff;
            Data.AlumniData = alumni;
            Data.AllBasicInfoWithoutAlumni = basic;
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
