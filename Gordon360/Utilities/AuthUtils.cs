using Gordon360.Static.Names;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace Gordon360.Utilities
{
    public class AuthUtils
    {
        static PrincipalContext Context => new(ContextType.Domain);

        /// <summary>
        /// Get the username of the authenticated user
        /// </summary>
        /// <param name="User">The ClaimsPrincipal representing the user's authentication claims</param>
        /// <returns>Username of the authenticated user</returns>
        public static string GetAuthenticatedUserUsername(ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.Upn).Split("@")[0];
        }

        public static IEnumerable<string> GetAuthenticatedUserGroups(ClaimsPrincipal User)
        {
            var groups = User.Claims.Where(x => x.Type == "groups").Select(g => g.Value);
            return groups;
        }

        public static bool UserIsInGroup(ClaimsPrincipal User, AuthGroup group)
        {
            var groups = GetAuthenticatedUserGroups(User);
            return groups.Contains(group.Name);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Program only runs on Windows")]
        public static IEnumerable<string> GetGroups(string userName)
        {
            UserPrincipal user = UserPrincipal.FindByIdentity(Context, userName);

            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            return user.GetAuthorizationGroups()
                .Where(g => g is GroupPrincipal)
                .Select(g => g.SamAccountName);
        }
    }
}

