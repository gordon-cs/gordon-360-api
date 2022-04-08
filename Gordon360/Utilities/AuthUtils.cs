using Gordon360.Database.CCT;
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

        //public static int? GetUserID(ClaimsPrincipal User)
        //{
        //    string? username = GetUsername(User);
        //    if (username == null) return null;
        //    return new CCTContext().ACCOUNT.Where(a => a.AD_Username == username).Select(a => int.Parse(a.gordon_id)).FirstOrDefault();
        //}
    }
}
