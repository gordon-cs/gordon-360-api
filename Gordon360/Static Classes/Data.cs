using System.IO;
using System.Xml.Linq;

namespace Gordon360.Static.Data
{
    /// <summary>
    /// Service class for data that is shared between all services.
    /// </summary>
    public static class Data
    {
        public static MemoryStream AllEvents { get; set; }
    }
}