namespace Gordon360.Static.Methods;

/// <summary>
/// Profile item class contining a single profile datum and privacy flag.
/// </summary>
public record ProfileItem{
    public ProfileItem (object d, bool p) {
        value = d;
        isPrivate = p;
    }
    public object value { get; set; }
    public bool isPrivate { get; set ;}
}