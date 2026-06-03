using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class PosterUploadViewModel

{
    public string ACT_CDE { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string ImagePath { get; set; }
    public DateTime VisibleDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string UploaderADUsername { get; set; }
    public int StatusID { get; set; }
    public int? Priority { get; set; }

    public Poster ToPoster()
    {
        return new Poster
        {
            ACT_CDE = this.ACT_CDE,
            Title = this.Title,
            Description = this.Description,
            ImagePath = this.ImagePath,
            VisibleDate = this.VisibleDate,
            ExpirationDate = this.ExpirationDate,
            UploaderADUsername = this.UploaderADUsername,
            StatusID = 1, //default status
            Priority = this.Priority ?? 0
        };
    }
}
