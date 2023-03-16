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
            return dateTime.SpecifyUtc();
        return null;
    }

    /// <summary>
    /// Converts UTC DateTime to Eastern Standard Time 
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="dateTime">DateTime with kind UTC</param>
    /// <param name="timeZoneID">String timezoneinfo location</param>
    /// <returns>Converted DateTime with kind Unspecified</returns>
    static public DateTime ConvertFromUtc(this DateTime dateTime, string timeZoneID) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneID));


    /// <summary>
    /// Converts UTC DateTime? to Eastern Standard Time 
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="nullableDateTime">DateTime? with kind UTC</param>
    /// <param name="timeZoneID">String timezoneinfo location</param>
    /// <returns>Converted DateTime with kind Unspecified or null if parameter is null</returns>
    static public DateTime? ConvertFromUtc(this DateTime? nullableDateTime, string timeZoneID)
    {
        if (nullableDateTime is DateTime dateTime)
            return dateTime.ConvertFromUtc(timeZoneID);
        return null;
    }

    /// <summary>
    /// Converts Location of specified by TimeZoneInfoID DateTime to UTC
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="dateTime">Unspecified kind DateTime</param>
    /// <param name="timeZoneID">String timezoneinfo location</param>
    /// <returns>Converted DateTime with kind UTC</returns>
    static public DateTime ConvertToUtc(this DateTime dateTime, string timeZoneID) => TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneID));

    /// <summary>
    /// Converts Location of specified by TimeZoneInfoID DateTime to UTC
    /// List of time zones specified by TimeZoneInfo https://i.stack.imgur.com/zHzGt.png
    /// </summary>
    /// <param name="nullableDateTime">Unspecified kind DateTime?</param>
    /// <param name="timeZoneID">String timezoneinfo location</param>
    /// <returns>Converted DateTime with kind UTC</returns>
    static public DateTime? ConvertToUtc(this DateTime? nullableDateTime, string timeZoneID)
    {
        if (nullableDateTime is DateTime dateTime)
            return dateTime.ConvertToUtc(timeZoneID);
        return null;
    }
}