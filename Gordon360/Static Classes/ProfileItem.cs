namespace Gordon360.Static.Methods;

/// <summary>
/// Profile item class contining a single profile datum and privacy flag.
/// </summary>
public record ProfileItem<TValue>(TValue Value, bool IsPrivate);
