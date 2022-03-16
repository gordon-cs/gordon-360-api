using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Gordon360.Models.ViewModels
{

    public class EventViewModel
    {
        public string Event_ID { get; set; }
        public string Event_Name { get; set; }
        public string Event_Title { get; set; }
        public string Event_Type_Name { get; set; }
        public bool HasCLAWCredit { get; set; }
        public bool IsPublic { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }

        // Set the namespace for X Paths
        public static readonly XNamespace r25 = "http://www.collegenet.com/r25";

        // This view model contains pieces of info pulled from a JSon array which is pulled from 25Live, using a pre-defined function
        public EventViewModel(XElement eventDetails, XElement occurrenceDetails)
        {
            Event_ID = $"{eventDetails.Element(r25 + "event_id")?.Value}_{occurrenceDetails.Element(r25 + "reservation_id")?.Value}";
            Event_Name = eventDetails.Element(r25 + "event_name")?.Value;
            Event_Title = eventDetails.Element(r25 + "event_title")?.Value;
            Event_Type_Name = eventDetails.Element(r25 + "event_type_name")?.Value;
            Description = eventDetails.Elements(r25 + "event_text")?.FirstOrDefault(t => t.Element(r25 + "text_type_id")?.Value == "1")?.Element(r25 + "text")?.Value;
            Organization = eventDetails.Element(r25 + "organization")?.Element(r25 + "organization_name")?.Value;
            HasCLAWCredit = eventDetails.Elements(r25 + "category")?.Any(c => c.Element(r25 + "category_id")?.Value == "85") ?? false;
            IsPublic = eventDetails.Elements(r25 + "requirement")?.Any(r => r.Element(r25 + "requirement_id")?.Value == "3") ?? false;
            StartDate = occurrenceDetails.Element(r25 + "event_start_dt")?.Value;
            EndDate = occurrenceDetails.Element(r25 + "event_end_dt")?.Value;
            Location = occurrenceDetails.Element(r25 + "space_reservation")?.Element(r25 + "space")?.Element(r25 + "formal_name")?.Value;
        }

    }
}