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
    /// Specify the given DateTime? as UTC, without changing the time value.
    /// </summary>
    /// <param name="nullableDateTime"></param>
    /// <returns>dateTime with type UTC, or null</returns>
    static public DateTime? SpecifyUtc(this DateTime? nullableDateTime)
    {
        if (nullableDateTime is DateTime dateTime)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        return null;
    }

    /// <summary>
    /// Converts UTC DateTime to Eastern Standard Time 
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="dateTime">DateTime with kind UTC</param>
    /// <returns>Converted DateTime with kind Unspecified</returns>
    static public DateTime ConvertUtcToEst(this DateTime dateTime) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));


    /// <summary>
    /// Converts UTC DateTime? to Eastern Standard Time 
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="nullableDateTime">DateTime? with kind UTC</param>
    /// <returns>Converted DateTime with kind Unspecified or null if parameter is null</returns>
    static public DateTime? ConvertUtcToEst(this DateTime? nullableDateTime)
    {
        if (nullableDateTime is DateTime dateTime)
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        return null;
    }

    /// <summary>
    /// Converts Eastern Standard Time DateTime to UTC
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="dateTime">Unspecified kind DateTime</param>
    /// <returns>Converted DateTime with kind UTC</returns>
    static public DateTime ConvertEstToUtc(this DateTime dateTime) => TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

    /// <summary>
    /// Converts Eastern Standard Time DateTime? to UTC
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="nullableDateTime">Unspecified kind DateTime?</param>
    /// <returns>Converted DateTime with kind UTC</returns>
    static public DateTime? ConvertEstToUtc(this DateTime? nullableDateTime)
    {
        if (nullableDateTime is DateTime dateTime)
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        return null;
    }
}