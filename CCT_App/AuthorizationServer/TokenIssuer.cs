using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Diagnostics;

namespace Gordon360.AuthorizationServer
{
    public class TokenIssuer : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context)
        {
            // This call is required...
            // but we're not using client authentication, so validate and move on...
            await Task.FromResult(context.Validated());
        }


        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            // Get the user credentials
            var username = context.UserName;
            var password = context.Password;
            // Get service account credentials
            var serviceUsername = System.Web.Configuration.WebConfigurationManager.AppSettings["serviceUsername"];
            var servicePassword = System.Web.Configuration.WebConfigurationManager.AppSettings["servicePassword"];
            // Syntax like : my.server.com:8080 
            var ldapServer = System.Web.Configuration.WebConfigurationManager.AppSettings["ldapServer"];

            AuthenticationResult result;

            /*******************************
             * Ldap Authentication
             *******************************/
            try
            {
                PrincipalContext ADServiceConnection = new PrincipalContext(
                    ContextType.Domain,
                    ldapServer,
                    "OU=Gordon College,DC=gordon,DC=edu",
                    ContextOptions.Negotiate | ContextOptions.ServerBind | ContextOptions.SecureSocketLayer,
                    serviceUsername,
                    servicePassword);

                UserPrincipal userQuery = new UserPrincipal(ADServiceConnection);
                userQuery.SamAccountName = username;

                PrincipalSearcher search = new PrincipalSearcher(userQuery);
                UserPrincipal userEntry = (UserPrincipal)search.FindOne();
                search.Dispose();


                if (userEntry != null)
                {
                    /* Debugging Purposes */
                    Debug.WriteLine("\n\nFOUND!\n\n");
                    Debug.WriteLine(userEntry.DistinguishedName);
                    Debug.WriteLine(userEntry.SamAccountName);

                    /* End Debug */

                    PrincipalContext ADUserConnection = new PrincipalContext(
                        ContextType.Domain,
                        ldapServer,
                        "OU=Gordon College,DC=gordon,DC=edu"
                        );



                    var areValidCredentials = ADUserConnection.ValidateCredentials(
                        username,
                        password,
                        ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer
                        );

                    if (areValidCredentials)
                    {

                        var identity = new ClaimsIdentity(new[]
                            {
                            new Claim(ClaimTypes.Name, userEntry.Name),
                            new Claim(ClaimTypes.NameIdentifier, userEntry.EmployeeId)
                            },
                            context.Options.AuthenticationType
                        );
                        ADServiceConnection.Dispose();
                        result =  new AuthenticationResult();
                        context.Validated(identity);


                    }

                    ADServiceConnection.Dispose();
                    result =  new AuthenticationResult("Provided Credentials are not valid");
                }
                else
                {
                    Debug.WriteLine("\n\nNOT FOUND\n\n");
                    result =  new AuthenticationResult("User doesn't exist!");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception caught: " + e.ToString());
                result =  new AuthenticationResult("There was a connection problem");

            }
        }

        public class AuthenticationResult
        {
            public string ErrorMsg { get; private set; }
            public Boolean isSuccess => string.IsNullOrEmpty(ErrorMsg);

            public AuthenticationResult(string errMessage = null)
            {
                ErrorMsg = errMessage;
            }
        }

    }
}