using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Reflection;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/version")]
    [CustomExceptionFilter]
    [Authorize]
    public class VersionController : ApiController
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
                gitVersion = reader.ReadToEnd();
            }

            // Console.WriteLine("Version: {0}", gitVersion);
            //Console.WriteLine("Hit any key to continue");
            //Console.ReadKey();
            return Ok(gitVersion);
        }
    }
}