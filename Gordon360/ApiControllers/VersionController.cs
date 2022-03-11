using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Reflection;

namespace Gordon360.ApiControllers
{
    /// <summary>
    /// Get the short git SHA-1 and build date for the backend
    /// </summary>
    /// <returns>"Git Hash: {hashCode}; Build Time: {date and time}"</returns>
    /// <remarks></remarks>
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            string gitVersion = string.Empty;
            using (Stream stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Gordon360." + "version.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                gitVersion = reader.ReadLine();
            }
            return Ok(gitVersion);
        }
    }
}
