namespace Gordon360.Static.Methods;

/// <summary>
/// Profile item class contining a single profile datum and privacy flag.
/// </summary>
public record ProfileItem{
    public ProfileItem (object d, bool p) {
        data = d;
        isPrivate = p;
    }
    public object data { get; set; }
    public bool isPrivate { get; set ;}
}