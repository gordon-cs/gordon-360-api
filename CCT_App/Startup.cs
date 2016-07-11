using Owin;
using System.Web.Http;

namespace Gordon360
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure the options for the WebApi Component.
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);



        }
    }
}