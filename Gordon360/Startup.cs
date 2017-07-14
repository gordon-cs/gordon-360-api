using Owin;
using Microsoft.Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;
using Gordon360.AuthorizationServer;
using System.Web;
using System.Diagnostics;
using System;
using System.Net.Mail;
using System.Web.Caching;
using System.Net;


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


            // Configure the options for the WebApi Component.
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }

                    // Create a new dummy cache entry. We don't want to store anything here, because it will be gone on restart of application
                    // Thus, all we need is the frequent callback from this item
        private const string DummyCacheItemKey = "DeloresMichaelLindsay";

        // Create a dummy URL that never works
        private const string DummyPageURL = "http://localhost/TestCacheTimeout/WebForm1.aspx";

        // On the application start, register the cache entry. Pretty straightforward.
        protected void Application_Start(Object sender, EventArgs e)
        {
            RegisterCacheEntry();
        }


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
                DateTime.MaxValue, TimeSpan.FromMinutes(1),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));
            return true;
        }


        // Perform service
        private void DoWork()
        {
            Debug.WriteLine("DoWork(): " + DateTime.Now.ToString());
        }

        // Inside the callback we do all the service work
        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());
            DoWork();
            RegisterCacheEntry();
        }
    }
       
    }
