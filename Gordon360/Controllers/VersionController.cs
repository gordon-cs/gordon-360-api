using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gordon360.Controllers
{
    /// <summary>
    /// Get the short git SHA-1 and build date for the backend
    /// </summary>
    /// <returns>"Git Hash: {hashCode}; Build Time: {date and time}"</returns>
    /// <remarks></remarks>
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            var asm = typeof(VersionController).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return $"Git Hash: {attrs.FirstOrDefault(a => a.Key == "SourceRevisionId")?.Value}";
        }
    }
}
