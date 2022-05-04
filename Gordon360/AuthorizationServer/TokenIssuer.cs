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

        private static string LDAPServer => System.Web.Configuration.WebConfigurationManager.AppSettings["ldapServer"];
        private static string ServiceUsername => System.Web.Configuration.WebConfigurationManager.AppSettings["serviceUsername"];
        private static string ServicePassword => System.Web.Configuration.WebConfigurationManager.AppSettings["servicePassword"];

        public override async Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context)
        {
            // This call is required...
            // but we're not using client authentication, so validate and move on...
            await Task.FromResult(context.Validated());
        }

        // Someone should figure out where the await should go. Until then, I'm suppressing the warning because
        // it has been working just fine so far.
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task GrantResourceOwnerCredentials(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Password) || string.IsNullOrWhiteSpace(context.UserName))
            {
                context.SetError("Unsuccessful Login", "The username or password is not correct.");
                return;
            }

            try
            {
                using (PrincipalContext ADServiceConnection = new PrincipalContext(
                        ContextType.Domain,
                        LDAPServer,
                        $"DC=gordon,DC=edu",
                        ContextOptions.Negotiate | ContextOptions.ServerBind | ContextOptions.SecureSocketLayer,
                        ServiceUsername,
                        ServicePassword))
                {
                    var areValidCredentials = ADServiceConnection.ValidateCredentials(
                        context.UserName,
                        context.Password,
                        ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer
                        );

                    if (!areValidCredentials)
                    {
                        context.SetError("Unsuccessful Login", "The username or password is not correct");
                        return;
                    }

                    var userEntry = UserPrincipal.FindByIdentity(ADServiceConnection, IdentityType.SamAccountName, context.UserName);

                    var personID = userEntry.EmployeeId;
                    // Some accounts don't have id's 
                    if (personID == null)
                    {
                        context.SetError("Unsuccessful Login", "The username or password is not correct.");
                        return;
                    }

                    IUnitOfWork unitOfWork = new UnitOfWork();
                    var adminService = new AdministratorService(unitOfWork);
                    var accountService = new AccountService(unitOfWork);

                    var distinguishedName = userEntry.DistinguishedName;
                    var account = unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == personID);
                    var isAdmin = unitOfWork.AdministratorRepository.FirstOrDefault(x => x.ID_NUM.ToString() == personID);

                    var collegeRole = string.Empty;

                    if (account?.ReadOnly == 1)
                    {
                        collegeRole = Position.READONLY;
                    }
                    else if (account?.is_police == 1)
                    {
                        collegeRole = Position.POLICE;
                    }
                    else if (isAdmin != null)
                    {
                        collegeRole = Position.SUPERADMIN;
                    }
                    else if (distinguishedName.EndsWith("OU=Students,OU=Gordon College,DC=gordon,DC=edu") || context.UserName.ToLower() == "360.studenttest")
                    {
                        collegeRole = Position.STUDENT;
                    }
                    else if (distinguishedName.EndsWith("OU=Fac Users,OU=Faculty,OU=Gordon College,DC=gordon,DC=edu") || distinguishedName.EndsWith("OU=Staff Users,OU=Staff,OU=Gordon College,DC=gordon,DC=edu") || context.UserName.ToLower() == "360.stafftest" || context.UserName.ToLower() == "360.facultytest")
                    {
                        collegeRole = Position.FACSTAFF;
                    }
                    else if (distinguishedName.EndsWith("OU=Alumni,DC=gordon,DC=edu"))
                    {
                        collegeRole = Position.ALUMNI;
                    }
                    else
                    {
                        collegeRole = Position.DEFAULT;
                    }

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("name", userEntry.Name));
                    identity.AddClaim(new Claim("id", personID));
                    identity.AddClaim(new Claim("college_role", collegeRole));
                    identity.AddClaim(new Claim("user_name", context.UserName));
                    context.Validated(identity);
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