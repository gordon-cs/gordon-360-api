using Owin;
using Microsoft.Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;
using Gordon360.AuthorizationServer;
using Gordon360.Static.Data;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using System.Xml.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Caching;


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
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10),
                Provider = new TokenIssuer(),
                AccessTokenFormat = new CustomJWTFormat(issuer),
#if DEBUG
                AllowInsecureHttp = true
#endif

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

            // Register a job in the cache to re-occur at a specified interval
            RegisterEventCacheEntry();
            RegisterAccountCacheEntry();

            // Configure the options for the WebApi Component.
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }

        /// <summary>
        /// Caching task methods created using the article written by Omar Al Zabir
        /// Article: https://www.codeproject.com/Articles/12117/Simulate-a-Windows-Service-using-ASP-NET-to-run-sc
        /// </summary>
       
        // Create a new dummy cache entry. We don't want to store anything here, because it will be gone on restart of application
        // Thus, all we need is the frequent callback from this item
        private const string EventItemKey = "DeloresMichaelLindsay";
        private const string AccountItemKey = "DeathToGoDotGordon";

        // Register the entry in the cache
        private bool RegisterEventCacheEntry()
        {
            // Check and see if the dummy entry is already in the cache
            if (null != HttpRuntime.Cache[EventItemKey])
            {
                return false;
            }

            else
            {
                // Otherwise, we add it to the cache
                HttpRuntime.Cache.Add(EventItemKey, "Test", null,
                    DateTime.MaxValue, TimeSpan.FromMinutes(4),
                    CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(CacheEventRemovedCallback));
                return true;
            }

        }

        // Register the entry in the cache
        private bool RegisterAccountCacheEntry()
        {
            // Check and see if the dummy entry is already in the cache
            if (null != HttpRuntime.Cache[AccountItemKey])
            {
                return false;
            }

            else
            {
                // Otherwise, we add it to the cache
                HttpRuntime.Cache.Add(AccountItemKey, "Test", null,
                    DateTime.MaxValue, TimeSpan.FromMinutes(200),
                    CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(CacheAccountRemovedCallback));
                return true;
            }

        }

        // Perform a job (in this case, we are calling 25Live and storing the data in a "global" variable
        private void DoWork()
        {
            // Make a call to 25Live and retrieve a list of all events
            XDocument _memory = Helpers.GetLiveStream(URLs.ALL_EVENTS_REQUEST);
            if (_memory != null)
            {
                Data.AllEvents = _memory;
            }

        }

        private void DoMoreWork()
        {
            Data.AllBasicInfo = Helpers.GetAllBasicInfo();
        }

        // Inside the callback we do all the service work to cache events
        public void CacheEventRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            // Record that the callback works (output to debug console)
            Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());
            // Call the jobs you want to 
            DoWork();
            // Re-register the item in the cache
            RegisterEventCacheEntry();
        }

        // Inside the callback we do all the service work to cache accounts
        public void CacheAccountRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            // Record that the callback works (output to debug console)
            Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());
            // Call the jobs you want to 
            DoMoreWork();
            // Re-register the item in the cache
            RegisterAccountCacheEntry();
        }
    }
}
