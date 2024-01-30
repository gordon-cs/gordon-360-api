using System;

namespace Gordon360.Models.ViewModels;

public class AttendedEventViewModel
{
    public string LiveID { get; set; }
    public DateTime? CHDate { get; set; }
    public string CHTermCD { get; set; }
    public int? Required { get; set; }
    public string Event_Name { get; set; }
    public string Event_Title { get; set; }
    public string Description { get; set; }
    public string Organization { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Location { get; set; }

    // We're gonna take an eventviewmodel (info from 25Live) and a Chapeleventviewmodel (info form our database) 
    // then mash 'em together
    public AttendedEventViewModel(EventViewModel? a, ChapelEventViewModel b)
    {
        // First the EventViewModel
        LiveID = b.LiveID;
        CHDate = b.CHDate is not null && b.CHTime is not null ? b.CHDate.Value.Add(b.CHTime.Value.TimeOfDay) : null;
        CHTermCD = b.CHTermCD.Trim();
        Required = b.Required;

        Event_Name = a?.Event_Name ?? "";
        Event_Title = a?.Event_Title ?? "";
        Description = a?.Description ?? "";
        Organization = a?.Organization ?? "";
        StartDate = a?.StartDate ?? "";
        EndDate = a?.EndDate ?? "";
        Location = a?.Location ?? "";

    }
}