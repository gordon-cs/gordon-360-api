using System;
namespace Gordon360.Extensions.System;

static public class DateTimeExtensions
{
    /// <summary>
    /// Helper method that casts given date time to type UTC without changing the time
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>dateTime with type UTC</returns>
    static public DateTime SpecifyUtc(this DateTime dateTime) => DateTime.SpecifyKind((DateTime)dateTime, DateTimeKind.Utc);
    

    /// <summary>
    /// Function overload for nullable datetime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>dateTime with type UTC, if dateTime is null, will return null</returns>
    static public DateTime? SpecifyUtc(this DateTime? dateTime)
    {
        if (dateTime is null) return null;
        return DateTime.SpecifyKind((DateTime)dateTime, DateTimeKind.Utc);
    }
}
