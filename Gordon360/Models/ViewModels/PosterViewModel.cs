using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class PosterViewModel
{
    public int ID { get; set; }
    public string ClubCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public DateTime VisibleDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Status { get; set; }
    public int Priority { get; set; }


    public static implicit operator PosterViewModel(Poster p)
    {
        PosterViewModel vm = new PosterViewModel
        {
            ID = p.ID,
            ClubCode = p.ACT_CDE.Trim(),
            Title = p.Title,
            Description = p.Description ?? "",
            ImagePath = p.ImagePath,
            VisibleDate = p.VisibleDate,
            ExpirationDate = p.ExpirationDate,
            Status = p.Status.Status,
            Priority = p.Priority
        };

        return vm;
    }
}