
using System.Xml.Linq;
namespace Gordon360.Static.Data
{
    /// <summary>
    /// Service class for data that is shared between all services.
    /// </summary>
    public static class Data
    {
        // XDocument containing the XML data (parsed) from a 25Live URL
        public static XDocument AllEvents { get; set; }
    }
}