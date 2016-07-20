using Owin;
using Microsoft.Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;

using Gordon360.AuthorizationServer;
using System.IdentityModel.Tokens;
using System.Diagnostics;

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
    }
}