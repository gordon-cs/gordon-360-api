using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using Gordon360.Models.ViewModels;


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
        public ActionResult<VersionViewModel> Get()
        {
            var asm = typeof(VersionController).Assembly;
            var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
            return new VersionViewModel
            {
                GitHash = attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value,
                BuildTime = attrs.FirstOrDefault(a => a.Key == "BuildTime")?.Value,
            };
        }
    }
}
