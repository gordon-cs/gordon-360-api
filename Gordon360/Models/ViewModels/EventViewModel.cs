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
        public string Category_Id { get; set; }
        public List<EventCategory> Categories { get; set; }
        public string Requirement_Id { get; set; }
        public List<EventRequirement> Requirements { get; set; }
        public string Description { get; set; }
        public List<Object[]> Occurrences { get; set; }
        public string Organization { get; set; }

        // Set the namespace for X Paths
        private XNamespace r25 = "http://www.collegenet.com/r25";

        // This view model contains pieces of info pulled from a JSon array which is pulled from 25Live, using a pre-defined function
        public EventViewModel(XElement a)
        {
            Event_ID = a.Element(r25 + "event_id").ElementValueNull().Value;
            Event_Name = a.Element(r25 + "event_name").ElementValueNull().Value;
            Event_Title = a.Element(r25 + "event_title").ElementValueNull().Value;
            Event_Type_Name = a.Element(r25 + "event_type_name").ElementValueNull().Value;

            Categories = a.Elements(r25 + "category").Select(category => new EventCategory
            {
                CategoryID = category.Element(r25 + "category_id").Value,
                CategoryName = category.Element(r25 + "category_name").Value
            }).ToList();

            // Check for a category ID of 85, and if that is true, return 85

            //foreach (XElement category in a.ElementValueNull().Descendants(r25 + "category"))
            //    {
            //     if (category.ElementValueNull().Element(r25 + "category_id").ElementValueNull().Value == "85")
            //        {
            //        Category_Id = "85";
            //        }
            //    }

            Requirements = a.Elements(r25 + "requirement").Select(requirement => new EventRequirement
            {
                RequirementID = requirement.Element(r25 + "requirement_id").Value,
                RequirementName = requirement.Element(r25 + "requirement_name").Value
            }).ToList();
            //(from requirement in a.Elements(r25 + "requirement") select Tuple.Create(requirement.Element(r25 + "requirement_id").Value, requirement.Element(r25 + "requirement_name").Value)).ToList();

            //foreach (XElement requirement in a.ElementValueNull().Descendants(r25 + "requirement"))
            //    {
            //    // Requirements.Add(requirement.ElementValueNull().Element(r25 + "requirement_id").ElementValueNull().Value);
            //     if (requirement.ElementValueNull().Element(r25 + "requirement_id").ElementValueNull().Value == "3")
            //        {
            //        Requirement_Id = "3";
            //        }
            //    }

            List<Object[]> placeholder = new List<object[]>();
            foreach (XElement reservation in a.Element(r25 + "profile").ElementValueNull().Descendants(r25 + "reservation"))
            {
                Object[] occurrence = new Object[]
                {
                    reservation.Element(r25 +"event_start_dt").ElementValueNull().Value,
                    reservation.Element(r25 + "event_end_dt").ElementValueNull().Value,
                    reservation.Element(r25 + "space_reservation").ElementValueNull().Element(r25 + "space").ElementValueNull().Element(r25 + "formal_name").ElementValueNull().Value
                };
                placeholder.Add(occurrence);
            }
            Occurrences = placeholder;
            Description = a.Element(r25 + "event_text").ElementValueNull().Element(r25 + "text").ElementValueNull().Value;
            Organization = a.Element(r25 + "organization").ElementValueNull().Element(r25 + "organization_name").ElementValueNull().Value;

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


}