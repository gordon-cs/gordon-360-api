using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Reflection;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers.Api
{
    /// <summary>
    /// Get the short git SHA-1 and build date for the backend
    /// </summary>
    /// <returns>"Git Hash: {hashCode}; Build Time: {date and time}"</returns>
    /// <remarks></remarks>
    // GET api/<controller>

    [RoutePrefix("api/version")]
    [CustomExceptionFilter]
    [Authorize]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
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