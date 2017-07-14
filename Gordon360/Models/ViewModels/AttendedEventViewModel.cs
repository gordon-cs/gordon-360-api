using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
    public class AttendedEventViewModel
    {
        public string CHEventID { get; set; }
        public Nullable<DateTime> CHDate { get; set; }
        public Nullable<DateTime> CHTime { get; set; }
        public string CHTermCD { get; set; }
        public Nullable<int> Required { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type_Name { get; set; }
        public string Description { get; set; }
        public string Organization { get; set; }
        public List<Object[]> Occurrences { get; set; }
        public string Category_ID { get; set; }

        // We're gonna take an eventviewmodel (info from 25Live) and a Chapeleventviewmodel (info form our database) 
        // then mash 'em together
        public AttendedEventViewModel(EventViewModel a, ChapelEventViewModel b)
        {   
            // First the EventViewModel
            CHEventID = b.CHEventID;
            CHDate = b.CHDate;
            CHTime = b.CHTime;
            CHTermCD = b.CHTermCD.Trim();
            Required = b.Required;

            // Then the CHapelEventViewModel
            if (a != null)
            {
                Event_Name = a.Event_Name ?? "";
                Event_Type_Name = a.Event_Type_Name ?? "";
                Category_ID = a.Category_Id ?? "";
                Description = a.Description ?? "";
                Organization = a.Organization ?? "";
                Occurrences = a.Occurrences;

            }
            // If it's null, fill it with empty strings so we don't crash
            else
            {
                Event_Name =  "";
                Event_Type_Name =  "";
                Category_ID =  "";
                Description =  "";
                Organization =  "";
                Occurrences = new List<object[]>();
            }
 
        }
    }


}