using System;
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
        public JToken Description { get; set; }
        public JToken Location { get; set; }
        public string Organization { get; set; }


        public EventViewModel(JObject a, int index, bool single)
        {
            if (single)
            {
                Event_ID = a.SelectToken("events.event.event_id._text").ToString().Trim();
                Event_Name = a.SelectToken("events.event.event_name._text").ToString().Trim();
                Event_Type_Name = a.SelectToken("events.event.event_type_name._text").ToString();
                Start_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_start_dt._text").ToString());
                End_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_end_dt._text").ToString());
                Description = a.SelectToken("events.event.event_text[0].text._text") ?? "No description available";
                Location = a.SelectToken("events.event.profile.reservation.space_reservation.space.formal_name._text") ?? "No location available";
                Organization = a.SelectToken("events.event.organization.organization_name._text").ToString().Trim();
            }
            else
            {
                Event_ID = a.SelectToken("events.event[" + index + "].event_id._text").ToString().Trim();
                Event_Name = a.SelectToken("events.event[" + index + "].event_name._text").ToString().Trim();
                Event_Type_Name = a.SelectToken("events.event[" + index + "].event_type_name._text").ToString();
                Start_Time = DateTime.Parse(a.SelectToken("events.event[" + index + "].profile.init_start_dt._text").ToString());
                End_Time = DateTime.Parse(a.SelectToken("events.event[" + index + "].profile.init_end_dt._text").ToString());
                Description = a.SelectToken("events.event[" + index + "].event_text[0].text._text") ?? "No description available";
                Location = a.SelectToken("events.event[" + index + "].profile.reservation.space_reservation.space.formal_name._text") ?? "No location available";
                Organization = a.SelectToken("events.event[" + index + "].organization.organization_name._text").ToString().Trim();
            }
        }
    }


}