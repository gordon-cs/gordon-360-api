using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
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
        public List<EventOccurence> Occurrences { get; set; }

        // We're gonna take an eventviewmodel (info from 25Live) and a Chapeleventviewmodel (info form our database) 
        // then mash 'em together
        public AttendedEventViewModel(EventViewModel a, ChapelEventViewModel b)
        {   
            // First the EventViewModel
            LiveID = b.LiveID;
            CHDate = b.CHDate.Value.Add(b.CHTime.Value.TimeOfDay);
            CHTermCD = b.CHTermCD.Trim();
            Required = b.Required;
            // Then the CHapelEventViewModel
            if (a != null)
            {
                Event_Name = a.Event_Name ?? "";
                Event_Title = a.Event_Title ?? "";
                Description = a.Description ?? "";
                Organization = a.Organization ?? "";
                Occurrences = a.Occurrences ?? null;

            }
            // If it's null, fill it with empty strings so we don't crash
            else
            {
                Event_Name =  "";
                Event_Title = "";
                Description =  "";
                Organization =  "";
                Occurrences = null;
            }
 
        }
    }


}