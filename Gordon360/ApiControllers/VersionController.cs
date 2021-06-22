using System.IO;
using System.Reflection;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gordon360.Controllers.Api
{
    /// <summary>
    /// Get the short git SHA-1 and build date for the backend
    /// </summary>
    /// <returns>"Git Hash: {hashCode}; Build Time: {date and time}"</returns>
    /// <remarks></remarks>
    [Route("api/version")]
    [CustomExceptionFilter]
    [Authorize]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            string gitVersion = String.Empty;
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
