using System;
using Newtonsoft.Json.Linq;

namespace Gordon360.Models.ViewModels
{
    public class AttendedEventViewModel
    {
        public string CHBarcode { get; set; }
        public string CHEventID { get; set; }
        public string CHCheckerID { get; set; }
        public Nullable<DateTime> CHDate { get; set; }
        public Nullable<DateTime> CHTime { get; set; }
        public string CHTermCD { get; set; }
        public Nullable<int> Required { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type_Name { get; set; }
        public string Description { get; set; }
        public string Occurrence { get; set; }
        public string Organization { get; set; }
        public string Category_ID { get; set; }

        // We're gonna take an eventviewmodel (info from 25Live) and a Chapeleventviewmodel (info form our database) 
        // then mash 'em together
        public AttendedEventViewModel(EventViewModel a, ChapelEventViewModel b)
        {   
            // First the EventViewModel
            CHBarcode = b.CHBarcode.Trim();
            CHEventID = b.CHEventID;
            CHCheckerID = b.CHCheckerID.Trim();
            CHDate = b.CHDate;
            CHTime = b.CHTime;
            CHTermCD = b.CHTermCD.Trim();
            Required = b.Required;
            // Then the CHapelEventViewModel
            Event_Name = a.Event_Name ?? "";
            Event_Type_Name = a.Event_Type_Name ?? "";
            Category_ID = a.Category_Id ?? "";
            Description = a.Description ?? "";
            Organization = a.Organization ?? "";
 
        }
    }


}