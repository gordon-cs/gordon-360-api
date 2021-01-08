using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Gordon360.Models.ViewModels.ExtensionMethods;


namespace Gordon360.Models.ViewModels
{

    public class EventViewModel
    {
        // Class element declarations
        public string Event_ID { get; set; }
        public string Event_Name { get; set; }
        public string Event_Title { get; set; }
        public string Event_Type_Name { get; set; }
        public List<EventCategory> Categories { get; set; }
        public List<EventRequirement> Requirements { get; set; }
        public string Description { get; set; }
        public List<EventOccurence> Occurrences { get; set; }
        public string Organization { get; set; }

        // Set the namespace for X Paths
        private readonly XNamespace r25 = "http://www.collegenet.com/r25";

        // This view model contains pieces of info pulled from a JSon array which is pulled from 25Live, using a pre-defined function
        public EventViewModel(XElement a)
        {
            Event_ID = a.Element(r25 + "event_id")?.Value;
            Event_Name = a.Element(r25 + "event_name")?.Value;
            Event_Title = a.Element(r25 + "event_title")?.Value;
            Event_Type_Name = a.Element(r25 + "event_type_name")?.Value;
            Description = a.Element(r25 + "event_text")?.Element(r25 + "text")?.Value;
            Organization = a.Element(r25 + "organization")?.Element(r25 + "organization_name")?.Value;

            Categories = a.Elements(r25 + "category")?.Select(category => new EventCategory
            {
                CategoryID = category.Element(r25 + "category_id")?.Value,
                CategoryName = category.Element(r25 + "category_name")?.Value
            }).ToList();

            Requirements = a.Elements(r25 + "requirement")?.Select(requirement => new EventRequirement
            {
                RequirementID = requirement.Element(r25 + "requirement_id")?.Value,
                RequirementName = requirement.Element(r25 + "requirement_name")?.Value
            }).ToList();

            Occurrences = a.Element(r25 + "profile")?.Descendants(r25 + "reservation").Select(o => new EventOccurence
            {
                StartDate = o.Element(r25 + "event_start_dt")?.Value,
                EndDate = o.Element(r25 + "event_end_dt")?.Value,
                Location = o.Element(r25 + "space_reservation")?.Element(r25 + "space")?.Element(r25 + "formal_name")?.Value
            }).ToList();
        }
    }

    public class EventCategory
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class EventRequirement
    {
        public string RequirementID { get; set; }
        public string RequirementName { get; set; }
    }

    public class EventOccurence
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
    }


}