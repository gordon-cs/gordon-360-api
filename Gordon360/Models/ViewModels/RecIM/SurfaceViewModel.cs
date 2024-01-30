using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels.RecIM;

public class SurfaceViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public static implicit operator SurfaceViewModel(Surface s)
    {
        return new SurfaceViewModel
        {
            ID = s.ID,
            Name = s.Name,
            Description = s.Description,
        };
    }
}