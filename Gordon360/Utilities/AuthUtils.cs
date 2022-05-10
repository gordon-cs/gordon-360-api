using Gordon360.Static.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Gordon360.Utilities
{
    public class AuthUtils
    {
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
    }
}

