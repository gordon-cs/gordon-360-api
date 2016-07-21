using System;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Diagnostics;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;

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
                        var personID = userEntry.EmployeeId;
                        // Some accounts don't have id's 
                        if (personID == null)
                        {
                            context.SetError("Unsuccessful Login", "The username or password is not correct.");
                            return;
                        }
                        var adminService = new AdministratorService(new UnitOfWork());
                        

                        var distinguishedName = userEntry.DistinguishedName;

                        var collegeRole = string.Empty;
                        if(distinguishedName.Contains("OU=Students"))
                        {
                            collegeRole = Position.STUDENT;
                        }
                        else
                        {
                            collegeRole = Position.FACSTAFF;
                        }
                        try
                        {
                            // This get operation is by gordon_id
                            // Throws an exception if not found.
                            var isAdmin = adminService.Get(personID);
                            if (isAdmin != null)
                            {
                                collegeRole = Position.GOD;
                            }
                        }
                        catch(ResourceNotFoundException e)
                        {
                            // Silent catch. 
                            // This is ok because we know this exception means the user is not an admin
                        }
                        
                        
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                        identity.AddClaim(new Claim("name", userEntry.Name));
                        identity.AddClaim(new Claim("id", personID));
                        identity.AddClaim(new Claim("college_role", collegeRole));

                        ADServiceConnection.Dispose();
                        context.Validated(identity);
                    }
                    else
                    {
                        ADServiceConnection.Dispose();
                        context.SetError("Unsuccessful Login", "The username or password is not correct");
                    }
                    
                    
                }
                else
                {
                    Debug.WriteLine("\n\nNOT FOUND\n\n");
                    context.SetError("Unsuccessful Login", "The username or password is not correct");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception caught: " + e.ToString());
                context.SetError("connection_error", "There was a problem connecting to the authorization server.");

            }
        }


    }
}