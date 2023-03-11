using System;
namespace Gordon360.Extensions.System;

static public class DateTimeExtensions
{
    /// <summary>
    /// Specify the given DateTime as UTC, without changing the time value.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>dateTime with type UTC</returns>
    static public DateTime SpecifyUtc(this DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);


    /// <summary>
    /// Function overload for nullable datetime
    /// </summary>
    /// <param name="nullableDateTime"></param>
    /// <returns>dateTime with type UTC, or null</returns>
    static public DateTime? SpecifyUtc(this DateTime? nullableDateTime)
    {
        if (nullableDateTime is DateTime dateTime)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        return null;
    }
}
