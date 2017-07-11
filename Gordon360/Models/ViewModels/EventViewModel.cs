using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

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

        // Set the namespace for XML Paths
        private XNamespace r25 = "http://www.collegenet.com/r25";

        // This view model contains pieces of info pulled from a JSon array which is pulled from 25Live, using a pre-defined function
        public EventViewModel(XElement a)
        {
            Event_ID = a.Element(r25 + "event_id").Value;
        }
    }


}