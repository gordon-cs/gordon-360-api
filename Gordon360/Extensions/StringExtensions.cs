using System;

namespace Gordon360.Extensions.System;

static public class StringExtensions
{
    static public bool EqualsIgnoreCase(this string string1, string string2) => string1.Equals(string2, StringComparison.OrdinalIgnoreCase);

    static public bool StartsWithIgnoreCase(this string string1, string string2) => string1.StartsWith(string2, StringComparison.OrdinalIgnoreCase);
}
