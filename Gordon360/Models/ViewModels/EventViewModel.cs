using System;
using System.IO;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gordon360.Models.ViewModels
{
    public class EventViewModel
    {

        public string Event_ID { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type_Name { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }


        public EventViewModel(JObject a)
        {

            Event_ID = a.SelectToken("events.event.event_id._text").ToString().Trim();
            Event_Name = a.SelectToken("events.event.event_name._text").ToString().Trim();
            Event_Type_Name = a.SelectToken("events.event.event_type_name._text").ToString();
            Start_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_start_dt._text").ToString());
            End_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_end_dt._text").ToString());
            // Description = a.SelectToken("events.event.event_text[1].text._text").ToString().Trim();
            Description = "TEST";

            //Location = a.SelectToken("events.event.approval.approval_name._text").ToString().Trim();
            Location = "TEST LOCATION";
            Organization = a.SelectToken("events.event.organization.organization_name._text").ToString().Trim();
 
        }
    }


}