using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace Gordon360.Authorization
{
    public class AuthUtils
    {
        static PrincipalContext Context => new(ContextType.Domain);

        /// <summary>
        /// Get the username of the authenticated user
        /// </summary>
        /// <param name="User">The ClaimsPrincipal representing the user's authentication claims</param>
        /// <returns>Username of the authenticated user</returns>
        public static string GetUsername(ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.Upn).Split("@")[0];
        }

        public static IEnumerable<AuthGroup> GetGroups(ClaimsPrincipal User)
        {
            return User.Claims
                .Where(x => x.Type == "groups")
                .Select(g => AuthGroupEnum.FromString(g.Value))
                .OfType<AuthGroup>();
        }

        public static bool UserIsInGroup(ClaimsPrincipal User, AuthGroup group)
        {
            return GetGroups(User).Contains(group);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Program only runs on Windows")]
        public static IEnumerable<AuthGroup> GetGroups(string userName)
        {
            UserPrincipal user = UserPrincipal.FindByIdentity(Context, userName);

            if (user == null)
            {
                return Enumerable.Empty<AuthGroup>();
            }

            return user.GetAuthorizationGroups()
                               .Where(g => g is GroupPrincipal)
                               .Select(g => AuthGroupEnum.FromString(g.SamAccountName))
                               .OfType<AuthGroup>();
        }
    }
}

