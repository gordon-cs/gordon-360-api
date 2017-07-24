using System.Xml.Linq;

namespace Gordon360.Models.ViewModels.ExtensionMethods
{
        public static class EventViewModelHelpers
        {
            //This method is to handle if element is missing
            public static XElement ElementValueNull(this XElement element)
            {
                if (element != null)
                    return element;

                XElement empty = new XElement("empty_element", "");
                return empty;
            }

            //This method is to handle if attribute is missing
            public static string AttributeValueNull(this XElement element, string attributeName)
            {
                if (element == null)
                    return "";
                else
                {
                    XAttribute attr = element.Attribute(attributeName);
                    return attr == null ? "" : attr.Value;
                }
            }
        }
}