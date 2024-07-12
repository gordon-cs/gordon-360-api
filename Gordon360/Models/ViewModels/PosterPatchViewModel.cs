using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class PosterPatchViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public DateTime? VisibleDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Status { get; set; }
    public int? Priority { get; set; }

}