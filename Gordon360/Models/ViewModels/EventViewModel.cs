using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
    public class EventViewModel
    {

        public string Event_ID { get; set; }
        public string Event_Name { get; set; }
        public string Event_Type_Name { get; set; }
        public string Category_Id { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public JToken Description { get; set; }
        public JToken Location { get; set; }
        public List<Object[]> Locations { get; set; }
        public string Organization { get; set; }


        // This view model contains pieces of info pulled from a JSon array which is pulled from 25Live, using a pre-defined function
        public EventViewModel(JObject a, int index, bool single)
        {   
            // Attempt to get info
            try
            {
                // If it is only one event, the route is handled differently, as there is no array containing it
                if (single)
                {
                    // Pull info using specific route locations. If the location of the info ever changes,
                    // the route here will need to change as well
                    Event_ID = a.SelectToken("events.event.event_id._text").ToString().Trim();
                    Event_Name = a.SelectToken("events.event.event_name._text").ToString().Trim();
                    Event_Type_Name = a.SelectToken("events.event.event_type_name._text").ToString();
                    Start_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_start_dt._text").ToString());
                    End_Time = DateTime.Parse(a.SelectToken("events.event.profile.init_end_dt._text").ToString());
                    // Some events don't have a description or location, or the location is private
                    Description = a.SelectToken("events.event.event_text[0].text._text") ?? "No description available";
                    Category_Id = a.SelectToken("events.event.category.category_id._text") == null ? String.Empty : a.SelectToken("events.event.category.category_id._text").ToString();
                    // Some events have more than one occurrence, so we want to check for that (if the reservation field is an array container, then there is more than one occurrence)
                    if (a.SelectToken("events.event.profile.reservation").Type != JTokenType.Array)
                    {
                        Location = a.SelectToken("events.event.profile.reservation.space_reservation.space.formal_name._text") ?? "No location available";
                    }
                    else
                    {
                        // Make a container for the occurrences
                        List<Object[]> list = new List<object[]>();
                        // Count the occurrences
                        int count = a.SelectToken("events.event.profile.reservation").Count();
                        // Loop through and parse out each occurrence into a new array, which is then added to a list
                        for (int y = 0; y < count; y++)
                        {
                            Object[] loc = new Object[]
                           {
                           DateTime.Parse(a.SelectToken("events.event.profile.reservation[" + y + "].event_start_dt._text").ToString()),
                           // Here we check if the path is null, and handle it if it is
                           a.SelectToken("events.event.profile.reservation[" + y + "].space_reservation.space.formal_name._text") == null ? String.Empty : a.SelectToken("events.event.profile.reservation[" + y + "].space_reservation.space.formal_name._text")
                           };
                            list.Add(loc);
                            Locations = list;
                        }

                    }
                    Organization = a.SelectToken("events.event.organization.organization_name._text").ToString().Trim();
                }
                // We know here that several events are contained within the array, so we loop through and parse them out
                else
                {
                    Event_ID = a.SelectToken("events.event[" + index + "].event_id._text").ToString().Trim();
                    Event_Name = a.SelectToken("events.event[" + index + "].event_name._text").ToString().Trim();
                    Event_Type_Name = a.SelectToken("events.event[" + index + "].event_type_name._text").ToString();
                    Start_Time = DateTime.Parse(a.SelectToken("events.event[" + index + "].profile.init_start_dt._text").ToString());
                    End_Time = DateTime.Parse(a.SelectToken("events.event[" + index + "].profile.init_end_dt._text").ToString());
                    Description = a.SelectToken("events.event[" + index + "].event_text[0].text._text") ?? "No description available";
                    Category_Id = a.SelectToken("events.event[" + index + "].category.category_id._text") == null ? String.Empty : a.SelectToken("events.event[" + index + "].category.category_id._text").ToString();
                    if (a.SelectToken("events.event[" + index + "].profile.reservation").Type != JTokenType.Array)
                    {
                        Location = a.SelectToken("events.event[" + index + "].profile.reservation.space_reservation.space.formal_name._text") ?? "No location available";
                    }
                    else
                    {
                        int count = a.SelectToken("events.event[" + index + "].profile.reservation").Count();
                        List<Object[]> list = new List<object[]>();
                        for (int y = 0; y < count; y++)
                        {
                            Object[] loc = new Object[]
                            {
                           DateTime.Parse(a.SelectToken("events.event[" + index + "].profile.reservation[" + y + "].event_start_dt._text").ToString()),
                           a.SelectToken("events.event[" + index + "].profile.reservation[" + y + "].space_reservation.space.formal_name._text") == null ? String.Empty : a.SelectToken("events.event[" + index + "].profile.reservation[" + y + "].space_reservation.space.formal_name._text")
                            };
                            list.Add(loc);
                            Locations = list;
                        }
                    }
                    Organization = a.SelectToken("events.event[" + index + "].organization.organization_name._text").ToString().Trim();
                }
            }
            // Catch any exceptions thrown by the selecttoken 
            catch (Exception e)
            {
                throw new Exception("Could not find event information using the path provided!", e);
            }
        }
    }


}