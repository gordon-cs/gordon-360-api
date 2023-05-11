using System.Linq;

namespace Gordon360.Utilities
{
    public static class Utils
    {
        public static bool In<T>(this T x, params T[] set)
        {
            return set.Contains(x);
        }
    }
}
