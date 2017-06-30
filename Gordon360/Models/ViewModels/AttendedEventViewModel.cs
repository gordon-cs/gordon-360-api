using System;
using System.IO;
using System.Text;
using System.Net;
using Newtonsoft.Json;
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
        public string Event_ID { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type_Name { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public JToken Description { get; set; }
        public JToken Location { get; set; }
        public string Organization { get; set; }

        Nullable<DateTime> NoTIme = new DateTime();
        public AttendedEventViewModel(EventViewModel a, ChapelEventViewModel b)
        {
            CHBarcode = b.CHBarcode.Trim();
            CHEventID = b.CHEventID;
            CHCheckerID = b.CHCheckerID.Trim();
            CHDate = b.CHDate ?? NoTIme;
            CHTime = b.CHTime;
            CHTermCD = b.CHTermCD.Trim();
            Required = b.Required;

            Event_ID = a.Event_ID;
            Event_Name = a.Event_Name;
            Event_Type_Name = a.Event_Type_Name;
            Start_Time = a.Start_Time;
            End_Time = a.End_Time;
            Description = a.Description;
            Location = a.Location;
            Organization = a.Organization;
 
        }
    }


}