using Gordon360.Models.Gordon360;

namespace Gordon360.Models.ViewModels.RecIM;

public class SurfaceUploadViewModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public Surface ToSurface()
    {
        this.Name ??= this.Description;
        this.Description ??= this.Name;
        return new Surface
        {
            Name = this.Name,
            Description = this.Description,
        };
    }
}